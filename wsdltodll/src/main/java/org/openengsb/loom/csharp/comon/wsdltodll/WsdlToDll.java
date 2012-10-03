package org.openengsb.loom.csharp.comon.wsdltodll;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileFilter;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.Comparator;
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

    private static final String CSC_EXE = "csc.exe";

    private static final String SVCUTIL_EXE = "svcutil.exe";

    /**
     * List of default pathes where to search for the installation of the .net framework.
     */
    private static final String[] DEFAULT_WSDL_PATHS = new String[]{
        System.getenv("ProgramFiles(x86)") + "\\Microsoft SDKs\\Windows\\",
        System.getenv("ProgramFiles") + "\\Microsoft SDKs\\Windows\\" };

    private static final String[] DEFAULT_CSC_PATHS = new String[]{
        System.getenv("windir") + "\\Microsoft.NET\\Framework64\\",
        System.getenv("windir") + "\\Microsoft.NET\\Framework\\" };

    /**
     * Location of the file.
     * 
     * @parameter default-Value=null expression="${CollectionType}"
     * 
     */
    private String collectionType;
    /**
     * Location of the file.
     * 
     * @parameter expression="${project.build.directory}"
     * @required
     */
    private File outputDirectory;
    /**
     * Location of the svcutil.exe command
     * 
     * @parameter default-Value=null expression="${wsdlExeFolderLocation}"
     */
    private File svcutilExeFolderLocation;
    /**
     * Location of the csc command.
     * 
     * @parameter default-Value=null expression="${cscFolderLocation}"
     */
    private File cscFolderLocation;
    /**
     * Location of the svcutil file
     * 
     * @parameter
     * @required
     */
    private List<String> svcutilLocations;

    /**
     * Namespace of the WSDL file. This should be the namespace in which a domain should be located.
     * 
     * @parameter
     * @required
     */
    private String namespace;

    private String list_type = "System.collections.Generic.List`1";
    private List<String> cspath = new ArrayList<String>();

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

    private List<String> enumerateLines(String filename) throws IOException {
        String fileString = readFileToString(new File(filename));
        String[] lines = fileString.split("\n");
        List<String> result = new LinkedList<String>();
        int openBrakets = 0;
        int start = 0;
        for (int i = 0; i < lines.length; i++) {
            if (lines[i].contains("{")) {
                if (openBrakets <= 0) {
                    start = i - 1;
                }
                openBrakets++;
            }
            if (lines[i].contains("}")) {
                openBrakets--;
                if (openBrakets <= 0) {
                    result.add(convertArrayAriaTOString(start, i, lines));
                }
            }
        }
        return result;
    }

    private String convertArrayAriaTOString(int start, int end, String[] lines) {
        String result = "";
        for (int i = start; i <= end; i++) {
            result += lines[i] + "\n";
        }
        return result;
    }

    /**
     * Search for similarities content in two files
     * 
     * @param filename1
     * @param filename2
     * @throws IOException
     */
    private void findSimularities(String filename1, String filename2)
        throws IOException {
        List<String> file1Block = enumerateLines(filename1);
        String file2 = readFileToString(new File(filename2));
        for (String string : file1Block) {
            if (file2.contains(string)) {
                String headerAttributes = file2.substring(0,
                    file2.indexOf(string));
                headerAttributes = headerAttributes.substring(headerAttributes
                    .lastIndexOf("}"));
                file2 = file2.replace(headerAttributes + string, "}");
            }
        }
        writeToFile(filename2, file2);
    }

    private void writeToFile(String filename, String filecontent)
        throws IOException {
        FileWriter fileWriter = new FileWriter(new File(filename));
        fileWriter.append(filecontent);
        fileWriter.close();
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
     * Windows mode for maven execution
     * 
     * @throws MojoExecutionException
     * @throws IOException
     */
    private void createDllFromWsdlUsingWindowsMode()
        throws MojoExecutionException {
        svcutilCommand();
        cscCommand();
    }

    private String findWsdlCommand() throws MojoExecutionException {
        if (svcutilExeFolderLocation != null) {
            return svcutilExeFolderLocation.getAbsolutePath();
        }
        for (File sdk : findAllInstalledSDKs(DEFAULT_WSDL_PATHS)) {
            File bindir = new File(sdk, "bin");
            File wsdlFile = new File(bindir, SVCUTIL_EXE);
            if (wsdlFile.exists()) {
                svcutilExeFolderLocation = wsdlFile;
                return wsdlFile.getAbsolutePath();
            }
        }
        throw new MojoExecutionException("unable to find " + SVCUTIL_EXE
                + " in paths " + Arrays.toString(DEFAULT_WSDL_PATHS) + "\n "
                + "You can specify the path manually by adding the argument \n"
                + " -DwsdlExeFolderLocation=\"C:\\path\\to\\wsdl.exe\"");
    }

    private String findCscCommand() throws MojoExecutionException {
        if (cscFolderLocation != null) {
            return cscFolderLocation.getAbsolutePath();
        }
        for (File sdk : findAllInstalledSDKs(DEFAULT_CSC_PATHS)) {
            File file = new File(sdk, CSC_EXE);
            getLog().info(
                "Trying to find " + CSC_EXE + " in "
                        + sdk.getAbsolutePath());
            if (file.exists()) {
                cscFolderLocation = file.getAbsoluteFile();
                return file.getAbsolutePath();
            }
        }
        throw new MojoExecutionException("unable to find " + CSC_EXE
                + " in paths " + Arrays.toString(DEFAULT_CSC_PATHS) + "\n "
                + "You can specify the path manually by adding the argument \n"
                + " -DcscFolderLocation=\"C:\\path\\to\\csc.exe\"");
    }

    private Collection<File> findAllInstalledSDKs(String[] paths) {
        Collection<File> result = new LinkedList<File>();
        for (String s : paths) {
            File[] findAllInstalledSDKs = findInstalledSDKs(s);
            result.addAll(Arrays.asList(findAllInstalledSDKs));
        }
        return result;
    }

    private File[] findInstalledSDKs(String path) {
        File file = new File(path);
        if (!file.exists()) {
            return new File[0];
        }
        File[] installedSDKs = file.listFiles(new FileFilter() {
            @Override
            public boolean accept(File pathname) {
                return pathname.isDirectory();
            }
        });
        Arrays.sort(installedSDKs, new Comparator<File>() {
            @Override
            public int compare(File o1, File o2) {
                return o2.getName().compareTo(o1.getName());
            }
        });
        return installedSDKs;
    }

    /**
     * Choose from the collectionType if an array or a list should be choose
     */
    private void SetCollectionType() {
        if (collectionType.isEmpty()
                || collectionType.toUpperCase().equals("ARRAY")) {
            list_type = "";
        } else {
            list_type = "/ct:\"System.Collections.Generic.List`1\"";
        }
    }

    /**
     * Search for the svcutil command and execute it when it is found
     * 
     * @throws IOException
     */
    private void svcutilCommand() throws MojoExecutionException {
        String cmd = findWsdlCommand();
        int i = 0;
        SetCollectionType();
        for (String location : svcutilLocations) {
            String outputFilename = new File(outputDirectory, namespace + (i++)
                    + ".cs").getAbsolutePath();
            String[] command = new String[]{ cmd, list_type, location,
                "/noConfig", "/out:" + outputFilename };
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
            cspath.add(outputFilename);
        }
        try {
            findSimularities(cspath.get(0), cspath.get(1));
        } catch (IOException e) {
            throw new MojoExecutionException(
                "Unable to remove double classes from theoutputfile", e);
        }
    }

    /**
     * Reads a file and add the new lines
     * 
     * @param file
     * @return
     * @throws IOException
     */
    private static String readFileToString(File file) throws IOException {
        BufferedReader bufferedReader = new BufferedReader(new FileReader(file));
        StringBuilder sb = new StringBuilder();
        String line;
        try {
            while ((line = bufferedReader.readLine()) != null) {
                sb.append(line);
                sb.append("\n");
            }
        } finally {
            bufferedReader.close();
        }
        return sb.toString();
    }

    /**
     * Search for the csc command and execute it when it is found
     */
    private void cscCommand() throws MojoExecutionException {
        String cscPath = findCscCommand();
        List<String> commandList = new LinkedList<String>(cspath);
        commandList.add(0, cscPath);
        commandList.add(1, "/target:library");
        commandList.add(2, "/out:" + namespace + ".dll");
        String[] command = commandList.toArray(new String[commandList.size()]);
        ProcessBuilder builder = new ProcessBuilder();
        builder.redirectErrorStream(true);
        builder.directory(outputDirectory);
        builder.command(command);
        try {
            executeACommand(builder.start());
        } catch (IOException e) {
            throw new MojoExecutionException("Error, while executing command: "
                    + Arrays.toString(command) + "\n", e);
        } catch (InterruptedException e) {
            throw new MojoExecutionException("Error, while executing command: "
                    + Arrays.toString(command) + "\n", e);
        }
    }

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

        if (last.contains("'") && last.contains(".cs")) {
            String filepath = last.split("'")[1];
            File file = new File(filepath);
            boolean moved = file.renameTo(new File(outputDirectory, file
                .getName()));
            if (!moved) {
                throw new MojoExecutionException("Unable to move file: "
                        + file.getAbsolutePath());
            }
            input += "Moving file " + file.getName() + " to " + cspath + "\n";
            getLog().info(input);
        }
        if (child.waitFor() != 0) {
            throw new MojoExecutionException(error);
        }
    }
}
