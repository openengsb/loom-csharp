﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Apache.NMS.ActiveMQ.OpenWire;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.ActiveMQ.Util;
using Apache.NMS.Util;

namespace Protocols.ActiveMQ
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
        public ActiveMQProtocol() {}
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
                Message = new ActiveMQTextMessage((String)message);
            else
                Message = (Command)message;
            return new ActiveMQProtocol(Message, -1);
        }
        public IProtocol ConvertToProtocol(byte[] Message)
        {
            return convert(Message);
        }
        private IProtocol convert(byte[] message)
        {
            try
            {
                return new ActiveMQProtocol(format.Unmarshal(new EndianBinaryReader(new MemoryStream((byte[])message.Clone()))), -1);
            }
            catch
            {
                return null;
            }
        }
        public bool Compaire(object protocol)
        {
            if (protocol is ActiveMQProtocol)
            {
                ActiveMQProtocol pr = (ActiveMQProtocol)protocol;
                return pr.Message.GetType().IsInstanceOfType(Message) && Message.GetType().IsInstanceOfType(pr.Message);
            }
            return false;
        }

        public byte[] getMessage()
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
        public bool getMoreBytes()
        {
            return Message == null;
        }
        public void RetrieveInfoFromReceivdeMessage(IProtocol receivedMessage)
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
                ((Response)Message).CorrelationId = ((ActiveMQProtocol)receivedMessage).Message.CommandId;

            if (Message is MessageDispatch)
            {
                MessageDispatch dispatcher = Message as MessageDispatch;
                ConsumerInfo consumerinfo = ((ActiveMQProtocol)receivedMessage).Message as ConsumerInfo;
                if (dispatcher.ConsumerId == null)
                    dispatcher.ConsumerId = consumerinfo.ConsumerId;

                if (dispatcher.Destination == null)
                    dispatcher.Destination = consumerinfo.Destination;
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