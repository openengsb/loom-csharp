
using OSBConnection;
using Renci.SshNet;
using SonaTypeDependencies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {

            log4net.Config.BasicConfigurator.Configure();
            SonatypeDependencyManager dm = new SonatypeDependencyManager("org.openengsb.framework", "openengsb-framework", "3.0.0-SNAPSHOT", "zip", null);
            String filename = dm.DownloadArtefactToFolder(System.IO.Path.GetTempPath());
            string name = dm.UnzipFile(filename, new FileInfo(filename).Directory.FullName);
            OpenEngSBConnection openengsb = new OpenEngSBConnection(name);
            openengsb.connectToOSBWithSSH();
            List<String> commands = new List<string>();
            commands.Add("feature:install openengsb-domain-example");
            commands.Add("feature:install  openengsb-ports-jms");
            commands.Add("feature:install  openengsb-ports-rs");
            foreach (String command in commands)
            {
                openengsb.executeCommand(command);
            }
            openengsb.closeConnection();
        }
    }
}