using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;
using Org.Openengsb.Loom.CSharp.Bridge.ETM;
using Org.Openengsb.Loom.CSharp.Bridge.ETM.TCP;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.ActiveMQ;
using log4net;
using System.Collections.Concurrent;
using System.Threading;
namespace Org.Openengsb.Loom.Csharp.Common.Bridge.Abstract
{
    public abstract class AETM : IETM
    {
        private ILog logging;
        private ETMList interationConfiguration;
        /// <summary>
        /// Every message gets a position. This position is bind to the socket
        /// </summary>
        protected IDictionary<int, int> receivedMessageposition = new ConcurrentDictionary<int, int>();
        private static Semaphore semaphore = new Semaphore(1, 1);
        public IDictionary<int, Dictionary<int, IProtocol>> ReceivedMessages { get; set; }
        public AETM(ILog log, ETMList interationConfiguration)
        {
            this.logging= log;
            this.interationConfiguration = interationConfiguration;
        }
      
        /// <summary>
        /// Handels the incoming messages from a socket.
        /// </summary>
        /// <param name="input">The received message from a socket</param>
        /// <param name="socketId">The socket number</param>
        protected void HandleActions(byte[] input, int socketId)
        {
            if (IsEmpty(input))
            {
                return;
            }
            byte[] message = input;
            IProtocol prot = null;
            InteractionMessage transactions = interationConfiguration.FindInteraction(message, socketId, out prot);
            if (prot == null)
            {
                return;
            }
            message = CopyArray(input, prot.GetMessage().Length);

/*            if (prot is ActiveMQProtocol)
            {
                logging.Info(((ActiveMQProtocol)prot).Message.GetType().Name + " on socket: " + socketId + " with commandID: " + ((ActiveMQProtocol)prot).Message.CommandId);
            }*/
            if (!ReceivedMessages.ContainsKey(socketId))
            {
                ReceivedMessages.Add(socketId, new Dictionary<int, IProtocol>());
            }
            semaphore.WaitOne();
            ReceivedMessages[socketId].Add(receivedMessageposition[socketId]++, prot);
            semaphore.Release();

            if (transactions.Responses == null || transactions.Responses.Count <= 0)
            {
                return;
            }
            SentResponses(transactions, socketId, prot);
            if (!IsEmpty(message))
            {
                HandleActions(message, socketId);
            }
            return;
        }

        /// <summary>
        /// Checks if an array is empty or has just 0's;
        /// </summary>
        /// <param name="array">array to check</param>
        /// <returns></returns>
        protected Boolean IsEmpty(Byte[] array)
        {
            return !array.Any(b => b != 0);
        }

        /// <summary>
        /// Copies all the array from a starting postion to the end
        /// </summary>
        /// <param name="source">array</param>
        /// <param name="start">start position</param>
        /// <returns>new array</returns>
        private byte[] CopyArray(Byte[] source, int start)
        {
            byte[] result = new byte[source.Length - start];
            for (int i = 0; i < source.Length - start; i++)
            {
                result[i] = source[start + i];
            }
            return result;
        }
        /// <summary>
        /// Add an Interaction to the list
        /// </summary>
        /// <param name="interactionMessage">Interaction Message</param>
        public void AddInteraction(InteractionMessage interactionMessage)
        {
            interationConfiguration.Add(interactionMessage);
        }

        public abstract void Start(System.Net.IPEndPoint endpoint);
        public abstract void Start(System.Net.IPAddress adresse, int port);
        public abstract void TriggerMessage(InteractionMessage messagetoSend);

        public abstract void Dispose();
        protected abstract void SentResponses(InteractionMessage interactuinmessage, int socket, IProtocol protocol);
    }
}