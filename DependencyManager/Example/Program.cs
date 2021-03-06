﻿#region Copyright
// <copyright file="Program.cs" company="OpenEngSB">
// Licensed to the Austrian Association for Software Tool Integration (AASTI)
// under one or more contributor license agreements. See the NOTICE file
// distributed with this work for additional information regarding copyright
// ownership. The AASTI licenses this file to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using OSBConnection;
using SonaTypeDependencies;
using UnzipArtifact;

namespace Example
{
    /// <summary>
    /// Example Program
    /// </summary>
    public class Program
    {
        private static String groupId = "org.openengsb.framework";
        private static String artifactId = "openengsb-framework";
        private static String version = "3.0.0-SNAPSHOT";
        private static String packaging = "zip";
        private static String classifier = null;

        /// <summary>
        /// Example code
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();
            SonatypeDependencyManager dm = new SonatypeDependencyManager(groupId, artifactId, version, packaging, classifier);
            FileInfo fileLocation = dm.DownloadArtifactToFolder(System.IO.Path.GetTempPath());
            IUnzipper unzipper = new SevenZipUnzipper(fileLocation);
            string unzipFileLocation = unzipper.UnzipFile(fileLocation.Directory.FullName);
            OpenEngSBConnection openengsb = new OpenEngSBConnection(unzipFileLocation);
            openengsb.StartOpenEngSB();
            openengsb.ConnectToOSBWithSSH();
            List<String> commands = new List<string>();
            commands.Add("feature:install openengsb-domain-example");
            commands.Add("feature:install openengsb-ports-jms");
            commands.Add("feature:install openengsb-ports-rs");
            foreach (String command in commands)
            {
                openengsb.ExecuteCommand(command);
            }

            openengsb.CloseConnection();
            openengsb.Shutdown();
        }
    }
}