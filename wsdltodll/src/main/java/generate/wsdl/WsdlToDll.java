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
	 * @required
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
	 * @param exec
	 *            List of paths, where the corresponding command could be
	 * @param command
	 *            Command to execute
	 * @param parameter
	 *            Parameters for the command
	 * @return true if a command has been executed correctly, false if not
	 * @throws MojoExecutionException
	 */
	private boolean executeCommand(List<String> exec, String command,
			String parameter) throws MojoExecutionException {

		for (int i = 0; i < exec.size(); i++) {
			String cmd = exec.get(i);
			cmd += "\\" + command;
			cmd = cmd.replace("\\\\", "\\");
			File commandlocation = new File(cmd);
			if (commandlocation.exists()) {
				try {

					String execute = "\"" + cmd + "\"" + " " + parameter;
					Runtime tr = Runtime.getRuntime();
					Process child = null;
					if (command.contains("wsdl"))
						// When the outputDirectory is indicated, then
						// /serviceInterface is transformed to
						// /serviceinterface,
						// which leads to an error
						child = tr.exec(execute);
					else
						child = tr.exec(execute, null, outputDirectory);

					// Print the result/error from the execution
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
					;
					// Because the wsdl.exe can not be executed in a
					// outputDirectory, the file has to be moved to the
					// corresponding folder
					if (last.contains("'")) {
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
					}
					if (child.waitFor() != 0)
						throw new MojoExecutionException(error);
					System.out.println(input);
					return true;
				} catch (IOException e) {
					throw new MojoExecutionException("unable to execute "
							+ command + " " + e);
				} catch (InterruptedException e) {
					throw new MojoExecutionException("unable to execute "
							+ command + " " + e);
				}
			}
		}
		return false;
	}

	/**
	 * Find and executes the commands wsdl.exe and csc.exe
	 */
	public void execute() throws MojoExecutionException {

		if (!outputDirectory.exists())
			outputDirectory.mkdirs();
		System.out.println("wsdllocation: " + wsdl_location);
		String wsdlparameter = "/serverInterface " + "\"" + wsdl_location
				+ "\"";
		if (!npanday_setting.exists())
			throw new MojoExecutionException(
					"npdanay-setting.xml could not be found: "
							+ npanday_setting);
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
		List<String> exec = new LinkedList<String>();
		List<String> sdk = new LinkedList<String>();
		for (int i = 0; i < nodes.getLength(); i++)
			sdk.add(nodes.item(i).getChildNodes().item(0).getNodeValue());
		nodes = document.getElementsByTagName("executablePath");
		for (int i = 0; i < nodes.getLength(); i++)
			exec.add(nodes.item(i).getChildNodes().item(0).getNodeValue());

		if (!executeCommand(sdk, "wsdl.exe", wsdlparameter))
			throw new MojoExecutionException("wsdl.exe could not be found");
		String cslocation = "/target:library \"" + cspath + "\"";
		if (!executeCommand(exec, "csc.exe", cslocation))
			throw new MojoExecutionException("csc.exe could not be found");

	}
}
