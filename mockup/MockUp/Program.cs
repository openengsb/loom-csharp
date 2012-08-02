using System;
using System.Collections.Generic;
using TCPHandling;
using log4net;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.ActiveMQ.OpenWire;
using Protocols.ActiveMQ;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects;
using ExampleDomain;
using System.Net;
using Protocols.ActiveMQConfiguration;
namespace MockUp
{
    class Program
    {
        private static ILog logger;
        static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();
            logger = LogManager.GetLogger(typeof(ETMTCP));
            logger.Info("The mock-up is going to be started, after pressing any key. When the ETM is running," +
                "you can invoke a method by pressing 1. Any other key will terminate the ETM");
            Console.ReadKey();
            logger.Info("START ETM");
            ETMTCP ETM = new ETMTCP(getETMConfiguration());
            ETM.Start(IPAddress.Loopback, 6549);
            readKey(ETM, 1);
            ETM.Dispose();
        }
        private static void readKey(ETMTCP ETM, int i)
        {
            if (Console.ReadKey().Key.Equals(ConsoleKey.D1))
            {
                logger.Info("Invoke doSomethingWithLogEvent");
                invokedoSomethingWithLogEvent(ETM, 8 + i);
                readKey(ETM, i + 1);
            }
            logger.Info("Close ETM");
            return;
        }
        private static void invokedoSomethingWithLogEvent(ETMTCP ETM, int socketID)
        {
            try
            {
                ETM.TriggerMessage(ActiveMQConfiguration.getNetBridgeAnswerInvoke(0, 0, ETM.ReceivedMessages, getdoSomethingWithLogEvent()));
                ETM.AddInteraction(ActiveMQConfiguration.getConsumerInfo(socketID));
                ETM.AddInteraction(ActiveMQConfiguration.getConnectionInfo(socketID));
                ETM.AddInteraction(ActiveMQConfiguration.getSendToConsumerVoidMessage(socketID, getBrideVoidAnswer()));
            }
            catch
            {
                logger.Info("Inpossible to invoke a method. Maybe the bridge isn't registered or started yer");
            }
        }
        private static List<InteractionMessage> getETMConfiguration()
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

            result.Add(ActiveMQConfiguration.getSendToConsumerVoidMessage(4, getBrideVoidAnswer()));
            result.Add(ActiveMQConfiguration.getSendToConsumerVoidMessage(7, getBrideVoidAnswer()));
            result.Add(ActiveMQConfiguration.getSendToConsumerVoidMessage(8, getBrideVoidAnswer()));
            return result;
        }
        private static String getBrideVoidAnswer()
        {
            return "{\"callId\":\"25069bc6-d3c5-4c96-8a92-3e5799316f4c\"" +
                ",\"timestamp\":" + new DateTime().Ticks + ",\"result\":{\"className\":null,\"arg\":null,\"type\":\"Void\"" +
            ",\"metaData\":{\"serviceId\":\"domain.example.events\",\"contextId\":\"foo\"}}}";
        }

        private static String getdoSomethingWithLogEvent()
        {
            IMarshaller marshaller = new JsonMarshaller();
            MethodCallMessage methodCall = new MethodCallMessage();
            methodCall.callId = "646bf4e9-1077-4188-b58a-787d610a135a";
            RemoteMethodCall remote = new RemoteMethodCall();
            remote.methodName = "doSomethingWithLogEvent";
            IDictionary<string, string> dic = new Dictionary<String, String>();
            List<Object> args = new List<Object>();
            LogEvent logevent = new LogEvent();
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

    }
}