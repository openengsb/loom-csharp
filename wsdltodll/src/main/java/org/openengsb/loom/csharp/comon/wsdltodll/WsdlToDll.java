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
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.maven.plugin.AbstractMojo;
import org.apache.maven.plugin.MojoExecutionException;

import sun.util.logging.resources.logging;

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
	 * List of default pathes where to search for the installation of the .net
	 * framework.
	 */
	private static final String[] DEFAULT_WSDL_PATHS = new String[] {
			System.getenv("ProgramFiles(x86)") + "\\Microsoft SDKs\\Windows\\",
			System.getenv("ProgramFiles") + "\\Microsoft SDKs\\Windows\\" };

	private static final String[] DEFAULT_CSC_PATHS = new String[] {
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
	private File wsdlExeFolderLocation;
	/**
	 * Location of the csc command.
	 * 
	 * @parameter default-Value=null expression="${cscFolderLocation}"
	 */
	private File cscFolderLocation;
	/**
	 * Location of the wsdl file
	 * 
	 * @parameter
	 * @required
	 */
	private List<String> wsdl_locations; // = new String[0];
	// private String wsdl_location;

	/**
	 * Namespace of the WSDL file. This should be the namespace in which a
	 * domain should be located.
	 * 
	 * @parameter
	 * @required
	 */
	private String namespace;

	private String list_type = "System.collections.Generic.List`1";
	private List<String> cspath = new ArrayList<String>();

	private List<String> handledClasses = new ArrayList<String>();

	/*
	 * private static final Pattern CLASS_START_PATTERN = Pattern.compile(
	 * "/// <remarks/>"+ "\\[System.CodeDom.Compiler.GeneratedCodeAttribute*\n"+
	 * "\\[System.SerializableAttribute*\n"+
	 * "\\[System.Diagnostics.DebuggerStepThroughAttribute*\n"+
	 * "\\[System.ComponentModel.DesignerCategoryAttribute*\n"+
	 * "\\[System.Xml.Serialization.XmlTypeAttribute\\(Namespace*\\]\n"+
	 * "*public partial class*\n",Pattern.MULTILINE);
	 */
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
	 * Windows mode for maven execution
	 * 
	 * @throws MojoExecutionException
	 */
	private void createDllFromWsdlUsingWindowsMode()
			throws MojoExecutionException {
		wsdlCommand();
		cscCommand();
	}

	private String findWsdlCommand() throws MojoExecutionException {
		if (wsdlExeFolderLocation != null) {
			return wsdlExeFolderLocation.getAbsolutePath();
		}
		for (File sdk : findAllInstalledSDKs(DEFAULT_WSDL_PATHS)) {
			File bindir = new File(sdk, "bin");
			File wsdlFile = new File(bindir, SVCUTIL_EXE);
			if (wsdlFile.exists()) {
				wsdlExeFolderLocation = wsdlFile;
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
	 * Search for the wsdl command and execute it when it is found
	 */
	private void wsdlCommand() throws MojoExecutionException {
		String cmd = findWsdlCommand();
		int i = 0;
		SetCollectionType();
		for (String location : wsdl_locations) {
			String outputFilename = new File(outputDirectory, namespace + (i++)
					+ ".cs").getAbsolutePath();
			String[] command = new String[] { cmd, list_type, location,
					"/out:" + outputFilename };
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
			try {
				readClassesFromFile(outputFilename);
			} catch (IOException e) {
				throw new MojoExecutionException(
						"unable to postprocess generated outputfile", e);
			}
			cspath.add(outputFilename);
		}

	}

	private Collection<String> findAllClassDefs(String fileString) {
		String[] parts = fileString.split("using");
		Collection<String> result = new HashSet<String>();
		for (String p : parts) {
			/*
			 * Matcher matcher = CLASS_START_PATTERN.matcher(p); if
			 * (!matcher.find()) { continue; }
			 */
			// getLog().info(p);
			// int index = matcher.start();
			String classDefString = "using" + p;

			result.add(classDefString);
		}
		return result;
	}

	private String replaceDuplicateClasses(final String fileString) {
		String result = fileString;
		for (String classDefString : findAllClassDefs(fileString)) {
			if (!handledClasses.contains(classDefString)) {
				handledClasses.add(classDefString);
			} else {
				result = result.replace(classDefString, "");
			}
		}
		return result;
	}

	private void readClassesFromFile(String outputFilename) throws IOException {
		String fileString = readFileToString(new File(outputFilename));
		String processed = replaceDuplicateClasses(fileString);
		if (processed.equals(fileString)) {
			return;
		}
		FileWriter fileWriter = new FileWriter(new File(outputFilename));
		fileWriter.append(processed);
		fileWriter.close();
	}

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
			input += "Moving file " + file.getName() + " to " + cspath + "\n";
			getLog().info(input);
		}
		if (child.waitFor() != 0) {
			throw new MojoExecutionException(error);
		}
	}
}
