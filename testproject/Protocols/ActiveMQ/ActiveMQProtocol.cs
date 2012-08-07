using System;
using System.IO;
using Apache.NMS.ActiveMQ.OpenWire;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.Util;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;

namespace Org.Openengsb.Loom.CSharp.Bridge.Protocol.ActiveMQ
{
    /// <summary>
    /// Simulation of the ActiveMQ protocol
    /// </summary>
    public class ActiveMQProtocol : IProtocol
    {
        #region Variables
        /// <summary>
        /// Command, which will be send or has been received
        /// </summary>
        public Command Message { get; set; }
        /// <summary>
        /// Socket number on which the message should be send or has been received
        /// </summary>
        public int SocketNumber { get; set; }
        /// <summary>
        /// Format, which activeMQ uses to marshall/unmarshall messages
        /// </summary>
        public static OpenWireFormat format;
#endregion
        #region Constructors
        /// <summary>
        /// Stores an Object and a Socketnumber. When the Object is a Wirewormat, then this
        /// object is used to configure the stored Wireformat
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="socketNumber">SocketNumber</param>
        public ActiveMQProtocol(Object obj, int socketNumber)
        {
            this.SocketNumber = socketNumber;
            this.Message = (Command)obj;            
            if (obj is WireFormatInfo)
            {
                format.RenegotiateWireFormat((WireFormatInfo)obj);
            }
        }
        #endregion
        #region Public Methods
        public IProtocol ConvertToProtocol(Object message)
        {
            if (message is String)
            {
                Message = new ActiveMQTextMessage((String)message);
            }
            else
            {
                Message = (Command)message;
            }
            return new ActiveMQProtocol(Message, -1);
        }
        public IProtocol ConvertToProtocol(byte[] Message)
        {
            try
            {
                return new ActiveMQProtocol(format.Unmarshal(new EndianBinaryReader(new MemoryStream((byte[])Message.Clone()))), -1);
            }
            catch
            {
                return null;
            }
        }
        public int CompareTo(IProtocol protocol)
        {
            if (protocol is ActiveMQProtocol)
            {
                ActiveMQProtocol pr = (ActiveMQProtocol)protocol;
                if (pr.Message.GetType().IsInstanceOfType(Message) && Message.GetType().IsInstanceOfType(pr.Message))
                {
                    return 1;
                }
            }
            return 0;
        }

        public byte[] GetMessage()
        {
            byte[] buffer = new byte[8192];
            MemoryStream mem = new MemoryStream(buffer);
            ///Normally ActiveMQ uses this command to marshall and send messages directly.
            ///To get the bytes, a Memorystream is used, which writes the messages in a byte array
            format.Marshal(Message, new EndianBinaryWriter(mem));
            Byte[] result = new Byte[mem.Position];
            Array.Copy(buffer, result, mem.Position);
            return result;
        }
        public bool GetMoreBytes()
        {
            return Message == null;
        }
        public void RetrieveInfoFromReceivedMessage(IProtocol receivedMessage)
        {
            if (receivedMessage is ActiveMQProtocol)
            {
                AddInfoToMessage(receivedMessage);
            }
        }
        /// <summary>
        /// Retrieves info from the recieved message and add it the the Message object
        /// </summary>
        /// <param name="receivedMessage">Message received from the socket</param>
        private void AddInfoToMessage(IProtocol receivedMessage)
        {
            if (Message is Response)
            {
                ((Response)Message).CorrelationId = ((ActiveMQProtocol)receivedMessage).Message.CommandId;
            }
            if (Message is MessageDispatch)
            {
                MessageDispatch dispatcher = Message as MessageDispatch;
                ConsumerInfo consumerinfo = ((ActiveMQProtocol)receivedMessage).Message as ConsumerInfo;
                if (dispatcher.ConsumerId == null)
                {
                    dispatcher.ConsumerId = consumerinfo.ConsumerId;
                }
                if (dispatcher.Destination == null)
                {
                    dispatcher.Destination = consumerinfo.Destination;
                }
                Message = dispatcher;
            }
        }
        /// <summary>
        /// Creates a copy of the protocol
        /// </summary>
        /// <returns>ActiveMQProtocol</returns>
        public object Clone()
        {
            return new ActiveMQProtocol(Message, SocketNumber);
        }
        #endregion
    }
}