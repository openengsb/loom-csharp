package org.openengsb.loom.csharp.comon.wsdltodll;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;

import org.apache.maven.plugin.AbstractMojo;
import org.apache.maven.plugin.MojoExecutionException;
import org.w3c.dom.Document;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import com.sun.org.apache.xerces.internal.parsers.DOMParser;

/**
 * Goal which creates a DLL from a WSDL file.
 * 
 * @goal run
 * 
 * @phase process-sources
 */
public class WsdlToDll extends AbstractMojo {

    /**
     * List of default pathes where to search for the installation of the .net framework.
     */
    private static final String[] DEFAULT_PATHES = new String[]{
        System.getenv("ProgramFiles(x86)") + "\\Microsoft SDKs\\Windows\\",
        System.getenv("ProgramFiles") + "\\Microsoft SDKs\\Windows\\", "C:\\Windows\\Microsoft.NET\\Framework64\\",
        "C:\\Windows\\Microsoft.NET\\Framework\\" };

    /**
     * Location of the file.
     * 
     * @parameter expression="${project.build.directory}"
     * @required
     */
    private File outputDirectory;
    /**
     * Location of the npanday-setting.xml.
     * 
     * @parameter expression="$M2_REPO\npanday-setting.xml"
     */
    private File npanday_setting;
    /**
     * Location of the wsdl file
     * 
     * @parameter expression="${base.dir}/target"
     * @required
     */
    private String wsdl_location;
    /**
     * Namespace of the WSDL file. This should be the namespace in which a domain should be located.
     * 
     * @parameter
     * @required
     */
    private String namespace;

    /**
     * Find and executes the commands wsdl.exe and csc.exe
     */
    @Override
    public void execute() throws MojoExecutionException {
        if (isWindows()) {
            createDllFromWsdlUsingWindowsMode();
        } else {
            createDllFromWsdlUsingLinuxMode();
        }
    }

    private boolean isWindows() {
        String os = System.getProperty("os.name");
        if (!outputDirectory.exists()) {
            outputDirectory.mkdirs();
        }
        getLog().info("Operation system:" + os);
        return os.toUpperCase().contains("WINDOWS");
    }

    /**
     * Linux mode for maven execution
     * 
     * @throws MojoExecutionException
     */
    private void createDllFromWsdlUsingLinuxMode() throws MojoExecutionException {
        // String wsdlparameter = "-server -n:" + createNamespace() + " \"" +
        // wsdl_location + "\"";
        String errorMessage = new StringBuilder().append(
            "========================================================================").append(
            "========================================================================").append(
            "This plugin can't be used under Linux").append(
            "========================================================================").append(
            "========================================================================").toString();
        throw new MojoExecutionException(errorMessage);
    }

    /**
     * Windows mode for maven execution
     * 
     * @throws MojoExecutionException
     */
    private void createDllFromWsdlUsingWindowsMode() throws MojoExecutionException {
        List<String> sdkandFrameworkPathes = new LinkedList<String>();
        if (npanday_setting == null || !npanday_setting.exists()) {
            if (npanday_setting != null)
                getLog().info(
                    "npdanay-setting.xml could not be found. Inidicated location:" + npanday_setting
                            + "Trying default pathes");
            else
                getLog().info("npdanay-setting.xml has been indicated. Trying default pathes");
        } else {
            getLog().info("Searching in the npanday file, for wsdl.exe and csc.exe");
            DOMParser parser = new DOMParser();
            try {
                parser.parse(npanday_setting.getAbsolutePath());
            } catch (SAXException e1) {
                throw new MojoExecutionException("Error while parsing the npanday_setting.xml \n" + e1);
            } catch (IOException e1) {
                throw new MojoExecutionException("Error while parsing the npanday_setting.xml \n" + e1);
            }
            Document document = parser.getDocument();
            NodeList nodes = document.getElementsByTagName("sdkInstallRoot");

            for (int i = 0; i < nodes.getLength(); i++) {
                sdkandFrameworkPathes.add(nodes.item(i).getChildNodes().item(0).getNodeValue());
            }
            nodes = document.getElementsByTagName("executablePath");
            for (int i = 0; i < nodes.getLength(); i++) {
                sdkandFrameworkPathes.add(nodes.item(i).getChildNodes().item(0).getNodeValue());
            }
            nodes = document.getElementsByTagName("installRoot");
            for (int i = 0; i < nodes.getLength(); i++) {
                sdkandFrameworkPathes.add(nodes.item(i).getChildNodes().item(0).getNodeValue());
            }
        }
        adddefaultSDKPath(sdkandFrameworkPathes);
        if (!wsdlCommand(sdkandFrameworkPathes)) {
            throw new MojoExecutionException("" + "wsdl.exe could not be found. Add "
                    + "<sdkInstallRoot>SDKPath/bin</sdkInstallRoot> "
                    + "to the NPanday file and configurate the plugin");
        }
        if (!cscCommand(sdkandFrameworkPathes)) {
            throw new MojoExecutionException("" + "csc.exe could not be found Add "
                    + "<executablePath>.NetFrameworkPath</executablePath> "
                    + "to the NPanday file and configurate the plugin");
        }
    }

    /**
     * Search for the wsdl command and execute it when it is found
     */
    private boolean wsdlCommand(List<String> possiblepathes) throws MojoExecutionException {
        for (String path : possiblepathes) {
            String cmd = path;
            if (cmd.lastIndexOf("\\") < path.length() - 1) {
                cmd += "\\";
            }
            cmd += "wsdl.exe";
            getLog().info("Trying path: " + cmd);
            if (new File(cmd).exists()) {
                String[] command = new String[]{ cmd, "/serverInterface", "/n:" + namespace, wsdl_location };
                ProcessBuilder builder = new ProcessBuilder();
                builder.redirectErrorStream(true);
                builder.command(command);
                try {
                    executeACommand(builder.start());
                } catch (IOException e) {
                    throw new MojoExecutionException("Error, while executing command: " + Arrays.toString(command)
                            + "\n", e);
                } catch (InterruptedException e) {
                    throw new MojoExecutionException("Error, while executing command: " + Arrays.toString(command)
                            + "\n", e);
                }
                return true;
            }
        }
        return false;
    }

    /**
     * Search for the csc command and execute it when it is found
     */
    private boolean cscCommand(List<String> possiblepathes) throws MojoExecutionException {
        for (String path : possiblepathes) {
            String cmd = path;
            if (cmd.lastIndexOf("\\") < path.length() - 1) {
                cmd += "\\";
            }
            cmd += "csc.exe";
            getLog().info("Trying " + cmd);
            if (!new File(cmd).exists()) {
                continue;
            }
            String[] command = new String[]{ cmd, "/target:library", cspath };
            ProcessBuilder builder = new ProcessBuilder();
            builder.redirectErrorStream(true);
            builder.directory(outputDirectory);
            builder.command(command);
            try {
                executeACommand(builder.start());
            } catch (IOException e) {
                throw new MojoExecutionException("Error, while executing command: " + Arrays.toString(command) + "\n",
                    e);
            } catch (InterruptedException e) {
                throw new MojoExecutionException("Error, while executing command: " + Arrays.toString(command) + "\n",
                    e);
            }
            return true;
        }
        return false;
    }

    String cspath;

    private void executeACommand(Process child) throws IOException, MojoExecutionException, InterruptedException {
        BufferedReader brout = new BufferedReader(new InputStreamReader(child.getInputStream()));
        String error = "", tmp, input = "", last = "";
        while ((tmp = brout.readLine()) != null) {
            input += tmp + "\n";
            last = tmp;
        }
        if (child.exitValue() > 0) {
            throw new MojoExecutionException(input);
        }
        // Because the wsdl.exe can not be executed in a outputDirectory, the
        // file has to be moved to the corresponding
        // folder
        if (last.contains("'") && last.contains(".cs")) {
            String filepath = last.split("'")[1];
            File file = new File(filepath);
            boolean moved = file.renameTo(new File(outputDirectory, file.getName()));
            if (!moved) {
                throw new MojoExecutionException("Unable to move file: " + file.getAbsolutePath());
            }
            cspath = outputDirectory + "\\" + file.getName();
            input += "Moving file " + file.getName() + " to " + cspath + "\n";
            getLog().info(input);
        }
        if (child.waitFor() != 0) {
            throw new MojoExecutionException(error);
        }
    }

    /**
     * Search in the default folder location of SDK and the .net Framework for the newest version. If the folder exist,
     * add it to the list
     */
    private void adddefaultSDKPath(List<String> exec) {
        for (String path : DEFAULT_PATHES) {
            File dir = new File(path);
            if (new File(path).exists()) {
                String[] children = dir.list();
                for (String folder : children) {
                    String fullpath = path + folder;
                    if (new File(fullpath).isDirectory()) {
                        if (new File(fullpath + "\\Bin\\").exists()) {
                            fullpath = fullpath + "\\Bin\\";
                        }
                        exec.add(fullpath);
                    }
                }
            }
        }
    }
}
