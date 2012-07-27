using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using TCPHandling;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Protocols.TCP;
using NUnit.Framework;
namespace ETMTest
{
    [TestFixture]
    public class ETMTCPTest
    {
        private List<InteractionMessage> workflow;
        private ETMTCP tcphanding;
        private byte[] message = null;
        private byte[] messageSendFromProgram;
        private byte[] messageSendFromMyFramework;
        private Socket socket;
        private int remoteSocketPort;
        private int localSocketPort;
        [TestFixtureSetUp]
        public void startup()
        {
            workflow = new List<InteractionMessage>();
            tcphanding = null;
            message = null;
            ASCIIEncoding asci = new ASCIIEncoding();
            messageSendFromProgram = asci.GetBytes("Hello World");
            messageSendFromMyFramework = asci.GetBytes("Hello World too");
            Random r = new Random();
            remoteSocketPort = ((int) (r.NextDouble() * 1000) + 1000);
            localSocketPort = ((int)(r.NextDouble() * 1000) + 1000);
        }

        [Test]
        public void TCPConnectionTest()
        {
            CreateTCPWorkflowReaction();
            StartMyFrameworkWithTCPProtocol();
            StartTCPComponentForTestingTCPConnection();

            Assert.AreEqual(message.Length, messageSendFromMyFramework.Length);
            for (int i = 0; i < message.Length; i++) Assert.AreEqual(message[i], messageSendFromMyFramework[i]);
        }

        #region Helperclasses
        private void CreateTCPWorkflowReaction()
        {
            //Descripe TCP Handling
            List<InteractionMessage> receiveMessage = new List<InteractionMessage>() { new InteractionMessage(localSocketPort, remoteSocketPort, new TCPProtocol(messageSendFromMyFramework,-1), null, IPAddress.Loopback, IPAddress.Loopback) };
            InteractionMessage SendMessage = new InteractionMessage(remoteSocketPort, localSocketPort, new TCPProtocol(messageSendFromProgram,-1), receiveMessage, IPAddress.Loopback, IPAddress.Loopback);

            workflow.Add(SendMessage);
            workflow.Sort();
        }
        private void StartMyFrameworkWithTCPProtocol()
        {
            tcphanding = new ETMTCP(workflow);
            tcphanding.Start(IPAddress.Loopback, localSocketPort);
        }
        private void StartTCPComponentForTestingTCPConnection()
        {
            InteractionMessage SendMessage = workflow[0];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, remoteSocketPort));
            socket.Connect(new IPEndPoint(IPAddress.Loopback, localSocketPort));
            ASCIIEncoding encoder = new ASCIIEncoding();
            socket.Send((Byte[])SendMessage.protocol.getMessage());

            message = new byte[4096];
            int bytesRead;
            bytesRead = socket.Receive(message, SocketFlags.None);

            Array.Resize(ref message, bytesRead);
        }
        [TestFixtureTearDown]
        public void Cleanup()
        {
            socket.Close();
            socket.Dispose();
            tcphanding.Dispose();
        }
    }
        #endregion
}