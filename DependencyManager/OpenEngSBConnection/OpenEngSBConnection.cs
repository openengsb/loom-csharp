using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Renci.SshNet;

namespace OSBConnection
{
    public class OpenEngSBConnection
    {
        private const String OpenEngSBBatLocation = @"\bin\openengsb.bat";
        private static ILog logger = LogManager.GetLogger(typeof(OpenEngSBConnection));
        private static int retries;
        private SshClient osbClient;
        private String osbFolderLocation = String.Empty;
        private String username = "karaf";
        private String password = "karaf";
        private String osbUrl = "localhost";
        private int port = 8101;
        private Process osbProcess = null;
        
        public int ExecutionTimeOutBetweenCommands { get; set; }

        public int TimeToWaitUntilOSBIsStarted { get; set; }

        public Boolean IsOSBConnectionOpen { get; private set; }

        public OpenEngSBConnection(String osbFolderLocation)
        {
            this.IsOSBConnectionOpen = false;
            this.osbFolderLocation = osbFolderLocation;
            retries = 5;
            this.ExecutionTimeOutBetweenCommands = 10000;
            this.TimeToWaitUntilOSBIsStarted = 5000;
        }

        public OpenEngSBConnection(String osbUrl, String username, String password, int port, String osbFolderLocation)
            : this(osbFolderLocation)
        {
            this.osbUrl = osbUrl;
            this.username = username;
            this.password = password;
            this.port = port;
        }

        public void StartOpenEngSB()
        {
            ProcessStartInfo start = new ProcessStartInfo(this.osbFolderLocation + "\\" + OpenEngSBBatLocation);
            start.WindowStyle = ProcessWindowStyle.Normal;
            start.CreateNoWindow = false;
            start.UseShellExecute = false;
            this.osbProcess = Process.Start(start);
            //// Wait until the OSB is started
            Thread.Sleep(this.TimeToWaitUntilOSBIsStarted);
        }

        public void ConnectToOSBWithSSH()
        {
            if (this.osbProcess == null)
            {
                throw new InvalidOperationException("The OSB process is not started");
            }

            this.IsOSBConnectionOpen = false;
            this.osbClient = new SshClient(this.osbUrl, this.port, this.username, this.password);
            try
            {
                this.osbClient.Connect();
            }
            catch (SocketException)
            {
                // The OSB is not started yet. Retry again
                if (retries-- <= 0)
                {
                    throw;
                }

                Thread.Sleep(this.TimeToWaitUntilOSBIsStarted);
                this.ConnectToOSBWithSSH();
            }

            this.IsOSBConnectionOpen = true;
        }

        public void ExecuteCommand(String command)
        {
            this.osbClient.RunCommand(command);
            Thread.Sleep(this.ExecutionTimeOutBetweenCommands);
        }

        public void CloseConnection()
        {
            this.IsOSBConnectionOpen = false;
            this.ExecuteCommand("shutdown -f");            
        }

        public void Shutdown()
        {
            if (IsOSBConnectionOpen)
            {
                CloseConnection();
            }

            Thread.Sleep(this.ExecutionTimeOutBetweenCommands);
            try
            {
                if (this.osbClient.IsConnected)
                {
                    this.osbProcess.Kill();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Killing proces did not work", ex);
            }

            this.osbClient.Disconnect();
        }
    }
}