using System;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using RuntimeTests.TestExceptionHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RuntimeTests.RuntimeTests
{
    //CHECK
    public class TestRuntimeJmsPort : OSBRunTimeTestParent
    {
        private const String tcpUrlOpenEngSB = "tcp://localhost.:6549?";
        private Guid tmpGuid;
        private const String connectorId = "TestCase";

        public override void Init()
        {
            TestCustomExceptionHandler.executions = 0;
            tmpGuid = Guid.NewGuid();
        }

        
        public void TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithIcomingPort()
        {
            string destination = tcpUrlOpenEngSB + tmpGuid.ToString();

            IIncomingPort inPort = new JmsIncomingPort(destination, new TestCustomExceptionHandler(), connectorId);
            inPort.Close();

            Assert.AreEqual<String>(inPort.Receive(), "TestCase");
        }

        public void TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithOutgoingPort()
        {
            string destination = tcpUrlOpenEngSB + tmpGuid.ToString();

            IOutgoingPort outPort = new JmsOutgoingPort(destination, new TestCustomExceptionHandler(), connectorId);
            outPort.Close();
            outPort.Send("Error");

            Assert.AreEqual<int>(TestCustomExceptionHandler.executions, 2);
        }

        public void TestJmsOutgoingPortWithQueueNameExceptionHandler()
        {
            Guid tmpGuid = Guid.NewGuid();
            string destination = tcpUrlOpenEngSB + tmpGuid.ToString();

            IOutgoingPort outPort = new JmsOutgoingPort(destination, new TestCustomExceptionHandler(), connectorId);
            outPort.Close();
            outPort.Send("Error", "NotExist");

            Assert.AreEqual<int>(TestCustomExceptionHandler.executions, 2);
        }

        public void TestJmsPortWithWrongUrlParameter()
        {
            string destination = "Wrong?" + tmpGuid.ToString();
            //The exceptHandler returns null and so the is connector is not stored.
            try
            {
                JmsPort outPort = new JmsIncomingPort(destination, new TestCustomExceptionHandler(), connectorId);
                Assert.Fail();
            }
            catch
            {
            }
        }
        public override void CleanUp()
        {
        }
    }
}