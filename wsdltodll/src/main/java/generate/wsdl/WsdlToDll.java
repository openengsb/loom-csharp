package generate.wsdl;

/*
 * Copyright 2001-2005 The Apache Software Foundation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

import org.apache.maven.plugin.AbstractMojo;
import org.apache.maven.plugin.MojoExecutionException;
import org.w3c.dom.Document;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import com.sun.org.apache.xerces.internal.parsers.DOMParser;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.LinkedList;
import java.util.List;

/**
 * Goal which touches a timestamp file.
 * 
 * @goal run
 * 
 * @phase process-sources
 */
public class WsdlToDll extends AbstractMojo {

    private String wsdlparameter;
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
     * 
     */
    private File npanday_setting;

    /**
     * Location of the wsdl fuke
     * 
     * @parameter expression="${base.dir}/target"
     * @required
     */
    private String wsdl_location;

    // Save the location of the cs file
    private String cspath;

    /**
     * 
     * @param exec List of paths, where the corresponding command could be
     * @param command Command to execute
     * @param parameter Parameters for the command
     * @return true if a command has been executed correctly, false if not
     * @throws IOException
     * @throws MojoExecutionException
     * @throws InterruptedException
     */
    private void exCommand(Process child) throws IOException, MojoExecutionException, InterruptedException {
        BufferedReader brout = new BufferedReader(
            new InputStreamReader(child.getInputStream()));
        BufferedReader br = new BufferedReader(
            new InputStreamReader(child.getErrorStream()));

        String error = "", tmp, input = "", last = "";
        while ((tmp = br.readLine()) != null)
            error += tmp;
        while ((tmp = brout.readLine()) != null) {
            input += tmp + "\n";
            last = tmp;
        }

        // Because the wsdl.exe can not be executed in a
        // outputDirectory, the file has to be moved to the
        // corresponding folder
        if (last.contains("'") && last.contains(".cs")) {
            String filepath = last.split("'")[1];
            File file = new File(filepath);
            boolean moved = file.renameTo(new File(outputDirectory,
                file.getName()));
            if (!moved)
                throw new MojoExecutionException(
                    "Unable to move file: "
                            + file.getAbsolutePath());
            cspath = outputDirectory + "\\" + file.getName();
            input += "Moving file " + file.getName() + " to "
                    + cspath + "\n";
            getLog().info(input);
        }
        if (child.waitFor() != 0)
            throw new MojoExecutionException(error);

    }

    private boolean wsdlCommand(List<String> possiblepathes, String parameter) throws MojoExecutionException {
        for (String path : possiblepathes) {
            String cmd = path;
            if (cmd.lastIndexOf("\\") < path.length() - 1)
                cmd += "\\";
            cmd += "wsdl.exe";
            if (new File(cmd).exists()) {
                String execute = "\"" + cmd + "\" " + parameter;
                getLog().info("trying " + cmd);
                Runtime tr = Runtime.getRuntime();
                try {
                    exCommand(tr.exec(execute));
                } catch (IOException e) {
                    throw new MojoExecutionException("Error, while executing command: " + execute + "\n", e);
                } catch (InterruptedException e) {
                    throw new MojoExecutionException("Error, while executing command: " + execute + "\n", e);
                }
                return true;
            }
        }
        return false;
    }

    private boolean cscCommand(List<String> possiblepathes, String parameter) throws MojoExecutionException {
        for (String path : possiblepathes) {
            String cmd = path;
            if (cmd.lastIndexOf("\\") < path.length() - 1)
                cmd += "\\";
            cmd += "csc.exe";
            if (new File(cmd).exists()) {
                String execute = "\"" + cmd + "\"" + " " + parameter;
                getLog().info("trying " + cmd);
                Runtime tr = Runtime.getRuntime();
                try {
                    exCommand(tr.exec(execute, null, outputDirectory));
                } catch (IOException e) {
                    throw new MojoExecutionException("Error, while executing command: " + execute + "\n", e);
                } catch (InterruptedException e) {
                    throw new MojoExecutionException("Error, while executing command: " + execute + "\n", e);
                }
                return true;
            }
        }
        return false;
    }

    private void LinuxMode() throws MojoExecutionException {
        throw new MojoExecutionException("This plugin can't be used under Linux");
    }

    private void AdddefaultSDKPath(List<String> exec) {
        String defaultpathes[] =
            new String[]{ "C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v7.0A\\Bin\\",
                "C:\\Program Files\\Microsoft SDKs\\Windows\\v7.0A\\Bin\\",
                "C:\\Windows\\Microsoft.NET\\Framework64\\v2.0.50727\\",
                "C:\\Windows\\Microsoft.NET\\Framework64\\v3.0\\",
                "C:\\Windows\\Microsoft.NET\\Framework64\\v3.5\\",
                "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\",
            };
        for (String string : defaultpathes) {
            if (new File(string).exists() && !exec.contains(string))                
                exec.add(string);
        }
    }

    private void WindowsMode() throws MojoExecutionException {
        List<String> sdkandFrameworkPathes = new LinkedList<String>();
        if (npanday_setting == null || !npanday_setting.exists()) {
            getLog().info("npdanay-setting.xml could not be found. Trying default pathes");
        }
        else {
            getLog().info("Searching in the npanday file, for wsdl.exe and csc.exe");
            DOMParser parser = new DOMParser();
            try {
                parser.parse(npanday_setting.getAbsolutePath());
            } catch (SAXException e1) {
                throw new MojoExecutionException(
                    "Error while parsing the npanday_setting.xml \n" + e1);
            } catch (IOException e1) {
                throw new MojoExecutionException(
                    "Error while parsing the npanday_setting.xml \n" + e1);
            }
            Document document = parser.getDocument();
            NodeList nodes = document.getElementsByTagName("sdkInstallRoot");

            for (int i = 0; i < nodes.getLength(); i++)
                sdkandFrameworkPathes.add(nodes.item(i).getChildNodes().item(0).getNodeValue());
            nodes = document.getElementsByTagName("executablePath");
            for (int i = 0; i < nodes.getLength(); i++)
                sdkandFrameworkPathes.add(nodes.item(i).getChildNodes().item(0).getNodeValue());
            nodes = document.getElementsByTagName("installRoot");
            for (int i = 0; i < nodes.getLength(); i++)
                sdkandFrameworkPathes.add(nodes.item(i).getChildNodes().item(0).getNodeValue());
        }
        AdddefaultSDKPath(sdkandFrameworkPathes);
        if (!wsdlCommand(sdkandFrameworkPathes, wsdlparameter))
            throw new MojoExecutionException(
                "wsdl.exe could not be found. Add "
                        + "<sdkInstallRoot>SDKPath/bin</sdkInstallRoot> to the NPanday file");

        String cslocation = "/target:library \"" + cspath + "\"";
        if (!cscCommand(sdkandFrameworkPathes, cslocation))
            throw new MojoExecutionException("csc.exe could not be found Add "
                    + "<executablePath>.NetFrameworkPath</executablePath> to the NPanday file");
    }

    /**
     * Find and executes the commands wsdl.exe and csc.exe
     */
    public void execute() throws MojoExecutionException {
        String os = System.getProperty("os.name");
        if (!outputDirectory.exists())
            outputDirectory.mkdirs();
        getLog().info("Operation system:" + os);
        String namespace = "defaultnamespace" + (int) (Math.random() * 100);
        if (wsdl_location.contains("-") && wsdl_location.contains("."))
            namespace = wsdl_location.substring(
                wsdl_location.lastIndexOf('-') + 1,
                wsdl_location.lastIndexOf('.'));

        if (os.toUpperCase().contains("WINDOWS")) {
            wsdlparameter = "/serverInterface /n:" + namespace + " \""
                    + wsdl_location + "\"";
            WindowsMode();
        }
        else {
            wsdlparameter = "-server -n:" + namespace + " \""
                    + wsdl_location + "\"";
            LinuxMode();
        }
    }
}
