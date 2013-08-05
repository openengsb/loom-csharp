using System;
using OSBConnection;
using SonaTypeDependencies;
using System.Collections.Generic;
using System.IO;

namespace RuntimeTests
{
    public abstract class OSBRunTimeTestParent
    {
        private static OpenEngSBConnection openengsb;
        private static String tmpFileLocation = System.IO.Path.GetTempPath();

        public abstract void Init();
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

        private static void DownloadAndStartOSB()
        {
            SonatypeDependencyManager dm = new SonatypeDependencyManager("org.openengsb.framework", "openengsb-framework", "3.0.0-SNAPSHOT", "zip", null);
            String filename = dm.DownloadArtefactToFolder(tmpFileLocation);
            string OpenEngSBFolder = dm.UnzipFile(filename, tmpFileLocation);
            openengsb = new OpenEngSBConnection(OpenEngSBFolder);
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
        public static void CloseOSB()
        {
            openengsb.closeConnection();
        }
        public abstract void CleanUp();
    }
}
