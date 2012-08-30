using System;
using System.Collections.Generic;
using Apache.NMS.ActiveMQ.Commands;
using System.Net;
using Org.Openengsb.Loom.CSharp.Bridge.ETM;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.ActiveMQ;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;
using Apache.NMS.ActiveMQ.OpenWire;

namespace AcitveMQProtocol.ActiveMQConfiguration
{
    public class ActiveMQConfiguration
    {
        public static List<InteractionMessage> getConfiguration()
        {
            return getConfiguration(getWireFormatInfo());
        }
        public static List<InteractionMessage> getConfiguration(WireFormatInfo wire)
        {
            List<InteractionMessage> result = new List<InteractionMessage>();
            result.Add(ActiveMQConfiguration.getRemoveInfoAnswer(-1));
            result.Add(ActiveMQConfiguration.getShutdownInfoAnswer(-1));
            result.Add(ActiveMQConfiguration.getKeepAliveAnswer(-1));
            result.Add(ActiveMQConfiguration.getWireFormatAnswer(wire, -1));
            result.Add(ActiveMQConfiguration.getNetBridgeTextMessageAnswer(-1));
            result.Add(ActiveMQConfiguration.getAskedAnswer(-1));
            result.Add(ActiveMQConfiguration.getSessionInfoAnswer(-1));
            result.Add(ActiveMQConfiguration.getProducerInfoAnswer(-1));
            result.Add(ActiveMQConfiguration.getConnectionInfoAnswer(-1));
            result.Add(ActiveMQConfiguration.getConsumerInfoAnswer(-1));
            return result;
        }
        public static MessageDispatch getDispatcher(Message message)
        {
            MessageDispatch dispatcher = new MessageDispatch();
            dispatcher.Message = message;
            MessageId messageId = new MessageId();
            messageId.BrokerSequenceId = 1;
            messageId.ProducerSequenceId = 1;
            dispatcher.Message.MessageId = messageId;
            return dispatcher;
        }
        public static InteractionMessage getAskedAnswer(int SocketID)
        {
            InteractionMessage activeMQMessage = new InteractionMessage(null, 6549, new ActiveMQProtocol(new MessageAck(), SocketID), null);
            return activeMQMessage;
        }

        public static InteractionMessage getNetBridgeInvokeMessageOnReceiveQueue(String messasge, IDictionary<int, Dictionary<int, IProtocol>> receivedMessages)
        {
            int answerSocket = 0;
            IProtocol prot = null;
            foreach (KeyValuePair<int, IProtocol> dic in receivedMessages[0])
            {
                if (dic.Value is ActiveMQProtocol && ((ActiveMQProtocol)dic.Value).Message is ConsumerInfo)
                {
                    prot = ((ActiveMQProtocol)dic.Value);
                }
            }
            prot.SocketNumber = 0;
            MessageDispatch dispatcher = getDispatcher(new ActiveMQTextMessage(messasge));
            dispatcher.Destination = new ActiveMQQueue("receive");
            InteractionMessage activeMQMessageAnswer = new InteractionMessage(6549, null, new ActiveMQProtocol(dispatcher, answerSocket), null);
            InteractionMessage activeMQMessage = new InteractionMessage(null, 6549, prot, new List<InteractionMessage>() { activeMQMessageAnswer });
            return activeMQMessage;
        }
        public static InteractionMessage getShutdownInfoAnswer(int SocketID)
        {
            InteractionMessage activeMQMessageresponse = new InteractionMessage(null, 6549, new ActiveMQProtocol(getResponse(),
                SocketID), null);
            InteractionMessage activeMQMessage = new InteractionMessage(null, 6549, new ActiveMQProtocol(new ShutdownInfo(),
                SocketID), new List<InteractionMessage>() { activeMQMessageresponse });
            return activeMQMessage;
        }
        public static InteractionMessage getRemoveInfoAnswer(int SocketID)
        {

            InteractionMessage activeMQMessageresponse = new InteractionMessage(null, 6549, new ActiveMQProtocol(getResponse(),
                SocketID), null, IPAddress.Loopback, IPAddress.Loopback);
            InteractionMessage activeMQMessage = new InteractionMessage(null, 6549, new ActiveMQProtocol(new RemoveInfo(),
                SocketID), new List<InteractionMessage>() { activeMQMessageresponse }, IPAddress.Loopback, IPAddress.Loopback);
            return activeMQMessage;
        }
        public static InteractionMessage getNetBridgeTextMessageAnswer(int socketID)
        {
            InteractionMessage activeMQMessageAnswer = new InteractionMessage(6549, null, new ActiveMQProtocol(getResponse(), socketID), null);
            InteractionMessage activeMQMessage = new InteractionMessage(null, 6549, new ActiveMQProtocol(new ActiveMQTextMessage(), socketID), new List<InteractionMessage>() { activeMQMessageAnswer });
            return activeMQMessage;
        }
        public static InteractionMessage getSendToConsumerVoidMessage(int socketID, String message)
        {
            MessageDispatch dispatcher = getDispatcher(new ActiveMQTextMessage(message));
            InteractionMessage activeMQMessageAnswer = new InteractionMessage(6549, null, new ActiveMQProtocol(dispatcher, socketID), null);
            InteractionMessage result = getConsumerInfoAnswer(socketID);
            result.Responses.Add(activeMQMessageAnswer);
            return result;
        }
        public static InteractionMessage getConsumerInfoAnswer(int socketID)
        {
            InteractionMessage consumerInfoAnswer = new InteractionMessage(6549, null, new ActiveMQProtocol(getResponse(), socketID), null);
            InteractionMessage consumerInfo = new InteractionMessage(null, 6549, new ActiveMQProtocol(new ConsumerInfo(), socketID), new List<InteractionMessage>() { consumerInfoAnswer });
            return consumerInfo;
        }
        public static InteractionMessage getProducerInfoAnswer(int socketID)
        {
            InteractionMessage producerInfoAnswer = new InteractionMessage(6549, null, new ActiveMQProtocol(getResponse(), socketID), null);
            InteractionMessage producerInfo = new InteractionMessage(null, 6549, new ActiveMQProtocol(new ProducerInfo(), socketID), new List<InteractionMessage>() { producerInfoAnswer });
            return producerInfo;
        }
        public static InteractionMessage getSessionInfoAnswer(int socketID)
        {
            InteractionMessage sessionInfoAnswer = new InteractionMessage(6549, null, new ActiveMQProtocol(getResponse(), socketID), null);
            InteractionMessage sessionInfo = new InteractionMessage(null, 6549, new ActiveMQProtocol(new SessionInfo(), socketID), new List<InteractionMessage>() { sessionInfoAnswer });
            return sessionInfo;
        }
        public static WireFormatInfo getWireFormatInfo()
        {
            ActiveMQProtocol.format = new OpenWireFormat();
            WireFormatInfo wire = new WireFormatInfo();
            wire.CommandId = 1;
            wire.Version = 6;
            wire.MaxInactivityDurationInitialDelay = 10000;
            wire.CacheSize = 0;
            wire.TcpNoDelayEnabled = false;
            wire.SizePrefixDisabled = false;
            wire.CacheEnabled = false;
            wire.MaxInactivityDuration = 30000;
            wire.StackTraceEnabled = false;
            wire.TightEncodingEnabled = false;
            return wire;
        }
        public static InteractionMessage getWireFormatAnswer(WireFormatInfo info, int socketID)
        {
            BrokerInfo broker = new BrokerInfo();
            BrokerId id = new BrokerId();
            id.Value = "ETM-" + socketID;
            broker.BrokerName = "ETM-Broker" + socketID;
            broker.BrokerURL = "tcp://localhost.:6549";
            broker.BrokerId = id;

            InteractionMessage brokerInfo = new InteractionMessage(6549, null, new ActiveMQProtocol(broker, socketID), null);
            InteractionMessage openConnectionAnswer = new InteractionMessage(6549, null, new ActiveMQProtocol(info, socketID), null);
            InteractionMessage openConnection = new InteractionMessage(null, 6549, new ActiveMQProtocol(info, socketID), new List<InteractionMessage>() { openConnectionAnswer, brokerInfo });
            return openConnection;
        }
        public static InteractionMessage getKeepAliveAnswer(int socketID)
        {
            InteractionMessage keepAliveAnswer = new InteractionMessage(6549, null, new ActiveMQProtocol(new KeepAliveInfo(), socketID), null);
            InteractionMessage keepAlive = new InteractionMessage(null, 6549, new ActiveMQProtocol(new KeepAliveInfo(), socketID), new List<InteractionMessage>() { keepAliveAnswer });
            return keepAlive;
        }
        public static InteractionMessage getConnectionInfoAnswer(int socketID)
        {
            InteractionMessage ConnectionInfoAnswer = new InteractionMessage(6549, null, new ActiveMQProtocol(getResponse(), socketID), null);
            InteractionMessage ConnectionInfo = new InteractionMessage(null, 6549, new ActiveMQProtocol(new ConnectionInfo(), socketID), new List<InteractionMessage>() { ConnectionInfoAnswer });
            return ConnectionInfo;
        }
        public static Response getResponse()
        {
            Response response = new Response();
            response.ResponseRequired = false;
            return response;
        }
    }
}
