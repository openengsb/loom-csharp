using System;
using System.Collections.Generic;
using System.Net;
using log4net;
using ExampleDomain;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.ActiveMQ.OpenWire;
using NUnit.Framework;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using AcitveMQProtocol.ActiveMQConfiguration;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;
using Org.Openengsb.Loom.CSharp.Bridge.ETM;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.ActiveMQ;
using Org.Openengsb.Loom.CSharp.Bridge.ETM.TCP;

namespace ETMTest
{
    [TestFixture]
    public class ActiveMQTesting
    {
        #region Variables
        private IETM ETM;        
        private ExampleDomainConnector localDomain;
        private string destination = "tcp://localhost.:6549";
        private string domainName = "example";
        IDomainFactory factory;
        #endregion
        [TestFixtureSetUp]
        public void startup()
        {
            localDomain = new ExampleDomainConnector();
        }
        [Test]
        public void TestBridge()
        {
            log4net.Config.BasicConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(IETM));
            logger.Info("START ETM");
            ETM = new ETMTCPImplementation(getETMConfiguration());
            ETM.Start(IPAddress.Loopback, 6549);
            startBridge();
            ETM.TriggerMessage(ActiveMQConfiguration.getNetBridgInvokeMessageOnReceiveQueue(0, 0, getTestCase(), ETM.ReceivedMessages));
            stopBridge();
            Assert.AreEqual(localDomain.message, "TestCase1");
            Assert.AreEqual(localDomain.level, "12");
            Assert.AreEqual(localDomain.name, "Test");
            Assert.AreEqual(localDomain.origin, "123");
            Assert.IsTrue(localDomain.processId == 123);
            Assert.AreEqual(localDomain.processIdSpecified, true);
        }
        #region Configuration
        private void startBridge()
        {
            factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, localDomain);
            factory.CreateDomainService(domainName);
            factory.RegisterConnector(factory.getServiceId(domainName), domainName);

            IExampleDomainEventsSoap11Binding remotedomain = factory.getEventhandler<IExampleDomainEventsSoap11Binding>(domainName);
            LogEvent lEvent = new LogEvent();
            lEvent.name = "Example";
            lEvent.level = "DEBUG";
            lEvent.message = "remoteTestEventLog";
            remotedomain.raiseEvent(lEvent);
        }
        private String getBrideVoidAnswer()
        {
            return "{\"callId\":\"25069bc6-d3c5-4c96-8a92-3e5799316f4c\"" +
                ",\"timestamp\":" + new DateTime().Ticks + ",\"result\":{\"className\":null,\"arg\":null,\"type\":\"Void\"" +
            ",\"metaData\":{\"serviceId\":\"domain.example.events\",\"contextId\":\"foo\"}}}";
        }

        private void stopBridge()
        {
            factory.UnRegisterConnector(domainName);
            factory.DeleteDomainService(domainName);
            factory.StopConnection(domainName);
        }
        private List<InteractionMessage> getETMConfiguration()
        {
            WireFormatInfo wire = ActiveMQConfiguration.getWireFormatInfo();
            ActiveMQProtocol.format = new OpenWireFormat();
            List<InteractionMessage> result = new List<InteractionMessage>() { };
            result.Add(ActiveMQConfiguration.getRemoveInfo(-1));
            result.Add(ActiveMQConfiguration.getShutdownInfo(-1));
            result.Add(ActiveMQConfiguration.getKeepAlive(-1));
            result.Add(ActiveMQConfiguration.getOpenConnection(wire, -1));
            result.Add(ActiveMQConfiguration.getNetBridgeTextMessageAnswer(-1));
            result.Add(ActiveMQConfiguration.getAsked(-1));
            result.Add(ActiveMQConfiguration.getSessionInfo(-1));
            result.Add(ActiveMQConfiguration.getProducerInfo(-1));

            for (int i = 0; i <= 9; i++)
            {
                result.Add(ActiveMQConfiguration.getConnectionInfo(i));
                result.Add(ActiveMQConfiguration.getConsumerInfo(i));
            }
            //Answer for the create MethodCall
            result.Add(ActiveMQConfiguration.getSendToConsumerVoidMessage(2, getBrideVoidAnswer()));
            //Answer for the register MethodCall
            result.Add(ActiveMQConfiguration.getSendToConsumerVoidMessage(4, getBrideVoidAnswer()));
            //Answer for the Invoke MethodCall
            result.Add(ActiveMQConfiguration.getSendToConsumerVoidMessage(6, getBrideVoidAnswer()));
            //Answer for the Unregister MethodCall
            result.Add(ActiveMQConfiguration.getSendToConsumerVoidMessage(7, getBrideVoidAnswer()));
            //Answer for the delete MethodCall
            result.Add(ActiveMQConfiguration.getSendToConsumerVoidMessage(9, getBrideVoidAnswer()));
            return result;
        }
        private String getTestCase()
        {
            IMarshaller marshaller = new JsonMarshaller();
            MethodCallMessage methodCall = new MethodCallMessage();
            methodCall.callId = "646bf4e9-1077-4188-b58a-787d610a135a";
            RemoteMethodCall remote = new RemoteMethodCall();
            remote.methodName = "doSomethingWithLogEvent";
            IDictionary<string, string> dic = new Dictionary<String, String>();
            List<Object> args = new List<Object>();
            LogEvent logevent = new LogEvent();
            localDomain.message = "TestCase1";
            logevent.level = "12";
            logevent.name = "Test";
            logevent.origin = "123";
            logevent.processId = 123;
            logevent.processIdSpecified = true;
            logevent.message = "TestCase1";
            args.Add(logevent);
            dic.Add("serviceId", "0e25f8d8-174b-470a-bc18-65c84c3df01a");
            remote.metaData = dic;
            remote.args = args;
            remote.classes = new List<String>() { "org.openengsb.domain.example.event.LogEvent" };
            remote.realClassImplementation = new List<String>() { "org.openengsb.domain.example.event.LogEvent" };
            methodCall.methodCall = remote;
            methodCall.answer = true;
            methodCall.destination = "tcp://localhost.:6549?0e25f8d8-174b-470a-bc18-65c84c3df01a";
            methodCall.principal = null;
            methodCall.credentials = null;
            methodCall.timestamp = new DateTime().Ticks;
            String msg = marshaller.MarshallObject(methodCall);
            return marshaller.MarshallObject(methodCall);
        }

        #endregion
        [TestFixtureTearDown]
        public void cleanup()
        {
            ETM.Dispose();
        }
    }
}