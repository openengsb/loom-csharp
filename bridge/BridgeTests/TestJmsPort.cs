using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using OOSourceCodeDomain;
using BridgeTests.TestConnectorImplementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using OpenEngSBCore;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using BridgeTests.TestExceptionHandlers;
namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestJmsPort
    {
        private const String tcpUrlOpenEngSB = "tcp://localhost.:6549?";
        private Guid tmpGuid;
        private const String connectorId = "TestCase";

        [TestInitialize]
        public void Initialise()
        {
            TestCustomExceptionHandler.executions = 0;
            tmpGuid = Guid.NewGuid();
        }

        [TestMethod]
        public void TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithIcomingPort()
        {
            string destination = tcpUrlOpenEngSB + tmpGuid.ToString();

            IIncomingPort inPort = new JmsIncomingPort(destination, new TestCustomExceptionHandler(), connectorId);
            inPort.Close();

            Assert.AreEqual<String>(inPort.Receive(), "TestCase");
        }

        [TestMethod]
        public void TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithOutgoingPort()
        {
            string destination = tcpUrlOpenEngSB + tmpGuid.ToString();

            IOutgoingPort outPort = new JmsOutgoingPort(destination, new TestCustomExceptionHandler(), connectorId);
            outPort.Close();
            outPort.Send("Error");

            Assert.AreEqual<int>(TestCustomExceptionHandler.executions, 2);
        }

        [TestMethod]
        public void TestJmsOutgoingPortWithQueueNameExceptionHandler()
        {
            Guid tmpGuid = Guid.NewGuid();
            string destination = tcpUrlOpenEngSB + tmpGuid.ToString();

            IOutgoingPort outPort = new JmsOutgoingPort(destination, new TestCustomExceptionHandler(), connectorId);
            outPort.Close();
            outPort.Send("Error", "NotExist");

            Assert.AreEqual<int>(TestCustomExceptionHandler.executions, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestJmsPortWithWrongUrlParameter()
        {
            string destination = "Wrong?" + tmpGuid.ToString();
            //The exceptHandler returns null and so the is connector is not stored.
            JmsPort outPort = new JmsIncomingPort(destination, new TestCustomExceptionHandler(), connectorId);
        }
    }
}