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

        [TestInitialize]
        public void Initialise()
        {
            TestCustomExceptionHandler.executions = 0;
        }
        [TestMethod]
        public void TestJmsIncomingPortExceptionHandler()
        {
            Guid tmpGuid = Guid.NewGuid();
            string destination = "tcp://localhost.:6549?" + tmpGuid.ToString();
            IIncomingPort inPort = new JmsIncomingPort(destination, new TestCustomExceptionHandler(), "TestCase");
            inPort.Close();
            Assert.AreEqual<String>(inPort.Receive(), "TestCase");
        }
        [TestMethod]
        public void TestJmsOutgoingPortExceptionHandler()
        {
            Guid tmpGuid = Guid.NewGuid();
            string destination = "tcp://localhost.:6549?" + tmpGuid.ToString();
            IOutgoingPort outPort = new JmsOutgoingPort(destination, new TestCustomExceptionHandler(), "TestCase");
            outPort.Close();
            outPort.Send("Error");
        }
        [TestMethod]
        public void TestJmsOutgoingPortWithQueueNameExceptionHandler()
        {
            Guid tmpGuid = Guid.NewGuid();
            string destination = "tcp://localhost.:6549?" + tmpGuid.ToString();
            IOutgoingPort outPort = new JmsOutgoingPort(destination, new TestCustomExceptionHandler(), "TestCase");
            outPort.Close();
            outPort.Send("Error", "NotExist");
        }
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestJmsPortWithWrongUrl()
        {
            Guid tmpGuid = Guid.NewGuid();
            string destination = "Wrong?" + tmpGuid.ToString();
            //The exceptHandler returns null and so the is connector is not stored.
            JmsPort outPort = new JmsIncomingPort(destination, new TestCustomExceptionHandler(), "TestCase");
        }
    }
}