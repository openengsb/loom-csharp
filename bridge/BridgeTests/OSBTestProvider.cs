using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSBConnection;
using SonaTypeDependencies;
using System.Collections.Generic;
using System.IO;

namespace BridgeTests
{
    public class OSBTestProvider
    {
        private static OpenEngSBConnection openengsb;
        private String tmpFileLocation = System.IO.Path.GetTempPath();
        protected void OpenOSB()
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

        private void DownloadAndStartOSB()
        {
            SonatypeDependencyManager dm = new SonatypeDependencyManager("org.openengsb.framework", "openengsb-framework", "3.0.0-SNAPSHOT", "zip", null);
            String filename = dm.DownloadArtefactToFolder(tmpFileLocation);
           // String filename = Path.Combine(tmpFileLocation,"openengsb-framework-3.0.0-20130519.084800-28.zip");
            string OpenEngSBFolder = dm.UnzipFile(filename, tmpFileLocation);
            openengsb = new OpenEngSBConnection(OpenEngSBFolder);
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
        private void CloseOSB()
        {
            openengsb.closeConnection();
        }
        protected void Cleanup()
        {
            CloseOSB();           
        }
    }
}
