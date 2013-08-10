#region Copyright
// <copyright file="OSBRunTimeTestParent.cs" company="OpenEngSB">
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

namespace RuntimeTests
{
    public abstract class OSBRunTimeTestParent
    {
        #region Constants
        public const String NullString = null;

        private const String GroupId = "org.openengsb.framework";
        private const String ArtifactId = "openengsb-framework";
        private const String Version = "3.0.0-SNAPSHOT";
        private const String Packaging = "zip";
        private const String Classifier = null;
        #endregion
        #region Private Static Variables
        private static OpenEngSBConnection openengsb;
        private static String tmpFileLocation = System.IO.Path.GetTempPath();
        #endregion
        #region Public Methods
        public static void CloseOSB()
        {
            openengsb.closeConnection();
        }

        public static void OpenOSB()
        {
            if (openengsb == null)
            {
                DownloadAndStartOSB();
            }
            else
            {
                openengsb.connectToOSBWithSSH();
            }
        }
        #endregion
        #region Abstract Methods
        public abstract void CleanUp();

        public abstract void Init();
        #endregion
        #region Private Methods
        private static void DownloadAndStartOSB()
        {
            SonatypeDependencyManager dm = new SonatypeDependencyManager(GroupId, ArtifactId, Version, Packaging, Classifier);
            String filename = dm.DownloadArtefactToFolder(tmpFileLocation);
            string openEngSBFolder = dm.UnzipFile(filename, tmpFileLocation);
            openengsb = new OpenEngSBConnection(openEngSBFolder);
            openengsb.ExecutionTimeOutBetweenCommands = 2500;
            openengsb.TimeToWaitUntilOSBIsStarted = 3000;
            openengsb.connectToOSBWithSSH();
            List<String> commands = new List<string>();
            commands.Add("feature:install openengsb-domain-example");
            commands.Add("feature:install  openengsb-ports-jms");
            commands.Add("feature:install  openengsb-ports-rs");
            foreach (String command in commands)
            {
                openengsb.executeCommand(command);
            }
        }
        #endregion
    }
}