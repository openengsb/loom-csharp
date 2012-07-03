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
        System.getenv("ProgramFiles") + "\\Microsoft SDKs\\Windows\\",
        "C:\\Windows\\Microsoft.NET\\Framework64\\",
        "C:\\Windows\\Microsoft.NET\\Framework\\" };

    /**
     * Location of the file.
     * 
     * @parameter expression="${project.build.directory}"
     * @required
     */
    private File outputDirectory;
    /**
     * Location of the wsdl.exe command
     * 
     * @parameter expression=null
     */
    private File wsdlExeFolderLocation;
    /**
     * Location of the csc command.
     * 
     * @parameter expression=null
     */
    private File cscFolderLocation;
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
    private void createDllFromWsdlUsingLinuxMode()
        throws MojoExecutionException {
        // String wsdlparameter = "-server -n:" + createNamespace() + " \"" +
        // wsdl_location + "\"";
        String errorMessage = new StringBuilder()
            .append("========================================================================")
            .append("========================================================================")
            .append("This plugin can't be used under Linux")
            .append("========================================================================")
            .append("========================================================================")
            .toString();
        throw new MojoExecutionException(errorMessage);
    }

    /**
     * Checks if a File exists
     * 
     * @param file
     * @return
     */
    private boolean checkExistens(File file) {
        return file != null && file.exists();
    }

    /**
     * Windows mode for maven execution
     * 
     * @throws MojoExecutionException
     */
    private void createDllFromWsdlUsingWindowsMode()
        throws MojoExecutionException {
        List<String> sdkandFrameworkPathes = new LinkedList<String>();
        if (checkExistens(wsdlExeFolderLocation)) {
            getLog().info(
                "Add the specified location of wsdl.exe to the pathes. "
                        + wsdlExeFolderLocation.getAbsolutePath());
            sdkandFrameworkPathes.add(cscFolderLocation.getAbsolutePath());
        }
        if (checkExistens(cscFolderLocation)) {
            getLog().info(
                "Add the specified location of csc.exe to the pathes. "
                        + cscFolderLocation.getAbsolutePath());
            sdkandFrameworkPathes.add(cscFolderLocation.getAbsolutePath());
        }
        adddefaultSDKPath(sdkandFrameworkPathes);
        if (!wsdlCommand(sdkandFrameworkPathes)) {
            throw new MojoExecutionException(""
                    + "wsdl.exe could not be found. Add "
                    + "<sdkInstallRoot>SDKPath/bin</sdkInstallRoot> "
                    + "to the NPanday file and configurate the plugin");
        }
        if (!cscCommand(sdkandFrameworkPathes)) {
            throw new MojoExecutionException(""
                    + "csc.exe could not be found Add "
                    + "<executablePath>.NetFrameworkPath</executablePath> "
                    + "to the NPanday file and configurate the plugin");
        }
    }

    /**
     * Search for the wsdl command and execute it when it is found
     */
    private boolean wsdlCommand(List<String> possiblepathes)
        throws MojoExecutionException {
        for (String path : possiblepathes) {
            String cmd = path;
            if (cmd.lastIndexOf("\\") < path.length() - 1) {
                cmd += "\\";
            }
            cmd += "wsdl.exe";
            getLog().info("Trying path: " + cmd);
            if (new File(cmd).exists()) {
                String[] command = new String[]{ cmd, "/serverInterface",
                    "/n:" + namespace, wsdl_location };
                ProcessBuilder builder = new ProcessBuilder();
                builder.redirectErrorStream(true);
                builder.command(command);
                try {
                    executeACommand(builder.start());
                } catch (IOException e) {
                    throw new MojoExecutionException(
                        "Error, while executing command: "
                                + Arrays.toString(command) + "\n", e);
                } catch (InterruptedException e) {
                    throw new MojoExecutionException(
                        "Error, while executing command: "
                                + Arrays.toString(command) + "\n", e);
                }
                return true;
            }
        }
        return false;
    }

    /**
     * Search for the csc command and execute it when it is found
     */
    private boolean cscCommand(List<String> possiblepathes)
        throws MojoExecutionException {
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
                throw new MojoExecutionException(
                    "Error, while executing command: "
                            + Arrays.toString(command) + "\n", e);
            } catch (InterruptedException e) {
                throw new MojoExecutionException(
                    "Error, while executing command: "
                            + Arrays.toString(command) + "\n", e);
            }
            return true;
        }
        return false;
    }

    String cspath;

    private void executeACommand(Process child) throws IOException,
        MojoExecutionException, InterruptedException {
        BufferedReader brout = new BufferedReader(new InputStreamReader(
            child.getInputStream()));
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
            boolean moved = file.renameTo(new File(outputDirectory, file
                .getName()));
            if (!moved) {
                throw new MojoExecutionException("Unable to move file: "
                        + file.getAbsolutePath());
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
