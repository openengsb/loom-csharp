using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;
using Org.Openengsb.Loom.CSharp.Bridge.ETM;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;

namespace Org.Openengsb.Loom.CSharp.Bridge.ETM.TCP
{
    /// <summary>
    /// This new implemenation of a list take into account the aready picked elements. Always the lowest
    /// choosen element will be returned, if the search criterias are the same
    /// </summary>
    class ETMList : IList
    {
        #region Variables
        private IDictionary<InteractionMessage, int> returnCounterForInteraciton = new ConcurrentDictionary<InteractionMessage, int>();
        private IList<InteractionMessage> interactionMessages = new List<InteractionMessage>();
        private List<IProtocol> protocolTypes = new List<IProtocol>();
        #endregion
        #region Constructors
        /// <summary>
        /// Adds a list of elements
        /// </summary>
        /// <param name="elements">Elements to add</param>
        public ETMList(IList<InteractionMessage> elements)
        {
            foreach (InteractionMessage element in elements)
            {
                this.Add(element);
            }
        }
        #endregion
        #region Public methods
        /// <summary>
        /// Will add the protocol type, when it isn't yet inside
        /// </summary>
        /// <param name="element">Element to add</param>
        public void AddProtocolType(InteractionMessage element)
        {
            IProtocol protocol = element.Protocol;
            if (!protocolTypes.Exists(pr => { return protocol.GetType().IsInstanceOfType(pr); }))
            {
                protocolTypes.Add(protocol);
            }
        }
        /// <summary>
        /// Search for an element and returns it when it could be found
        /// </summary>
        /// <param name="item">Search parameter</param>
        /// <param name="socketID">Socket, on which the message has been received</param>
        /// <returns>Found element</returns>
        public InteractionMessage SearchElement(IProtocol item, int socketID)
        {
            InteractionMessage canidates = null;
            int min = int.MaxValue;

            foreach (InteractionMessage message in interactionMessages)
            {
                if (CompaireProtocolAndInteractionmessage(item, socketID, message))
                {
                    // When there are two messages in a configuration with the same type then
                    // the choosen message will be returned and then will be the last for the next serch
                    if (returnCounterForInteraciton[message] < min)
                    {
                        canidates = message;
                        min = returnCounterForInteraciton[message];
                    }
                }
            }
            if (canidates != null)
            {
                returnCounterForInteraciton[canidates]++;
            }
            else
            {
                return null;
            }
            return canidates.Clone() as InteractionMessage;
        }
        /// <summary>
        /// Compaires if a protocol is the same as the protocol, specified in an interactionmessage.
        /// </summary>
        /// <param name="item">element1 to compaire</param>
        /// <param name="socketID">Socket number</param>
        /// <param name="message">interaction message</param>
        /// <returns>Compaire result</returns>
        private bool CompaireProtocolAndInteractionmessage(IProtocol item, int socketID, InteractionMessage message)
        {
            return (message.Protocol.SocketNumber == socketID || message.Protocol.SocketNumber < 0) && (message.Protocol.CompareTo(item) == 1);
        }
        /// <summary>
        /// Search in the list for an element, which is has the same type and the same has as configuration the same socket.
        /// </summary>
        /// <param name="message">received message from a socket</param>
        /// <param name="SocketId">the socket number on which the message has been received</param>
        /// <param name="convertedprotocol">Return value for the converted protocol</param>
        /// <returns>The search result</returns>
        public InteractionMessage FindInteraction(byte[] message, int SocketId, out IProtocol convertedprotocol)
        {
            IProtocol receivedmessage;
            Boolean moreData = false;
            convertedprotocol = null;
            foreach (IProtocol protocol in protocolTypes)
            {
                receivedmessage = protocol.ConvertToProtocol(message);
                if (IsProtocolValid(receivedmessage))
                {
                    InteractionMessage canidat = SearchElement(receivedmessage, SocketId);
                    if (canidat != null)
                    {
                        convertedprotocol = receivedmessage;
                        return (InteractionMessage)canidat.Clone();
                    }
                }
                else
                {
                    moreData = moreData || receivedmessage!=null && receivedmessage.GetMoreBytes() ;
                }
                
            }
            if (moreData)
            {
                return null;
            }
            throw new ArgumentException("It was impossible, to find a configuration for the indicated format");
        }

        private static bool IsProtocolValid(IProtocol receivedmessage)
        {
            if (receivedmessage != null)
            {
                return !receivedmessage.GetMoreBytes();
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Adds a element to the list
        /// </summary>
        /// <param name="value">Element to add</param>
        /// <returns>Position of the added element</returns>
        public int Add(object value)
        {
            InteractionMessage element = (InteractionMessage)value;
            interactionMessages.Add(element);
            returnCounterForInteraciton.Add(element, 0);
            AddProtocolType(element);
            return interactionMessages.Count - 1;
        }
        /// <summary>
        /// Deletes all the elements
        /// </summary>
        public void Clear()
        {
            returnCounterForInteraciton.Clear();
            interactionMessages.Clear();
        }
        /// <summary>
        /// Checks if the list contains the element
        /// </summary>
        /// <param name="value">element to check</param>
        /// <returns>if it contains the element</returns>
        public bool Contains(object value)
        {
            InteractionMessage element = (InteractionMessage)value;
            return interactionMessages.Contains(element);
        }
        /// <summary>
        /// returns Position of the element
        /// </summary>
        /// <param name="value">element to search</param>
        /// <returns>postion</returns>
        public int IndexOf(object value)
        {
            return interactionMessages.IndexOf((InteractionMessage)value);
        }
        /// <summary>
        /// Insert a element
        /// </summary>
        /// <param name="index">position</param>
        /// <param name="value">elment</param>
        public void Insert(int index, object value)
        {
            InteractionMessage element = (InteractionMessage)value;
            returnCounterForInteraciton.Add(element, 0);
            AddProtocolType(element);
            interactionMessages.Insert(index, element);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return interactionMessages.IsReadOnly; }
        }

        public void Remove(object value)
        {
            InteractionMessage element = value as InteractionMessage;
            interactionMessages.Remove(element);
            returnCounterForInteraciton.Remove(element);
        }

        public void RemoveAt(int index)
        {
            returnCounterForInteraciton.Remove(interactionMessages[index]);
            interactionMessages.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                return interactionMessages[index];
            }
            set
            {
                AddProtocolType((InteractionMessage)value);
                interactionMessages[index] = (InteractionMessage)value;
            }
        }
        public void CopyTo(Array array, int index)
        {
            if (array is InteractionMessage[])
            {
                interactionMessages.CopyTo(array as InteractionMessage[], index);
            }
            else
            {
                throw new ArgumentException("The array is not a InteractionMessage");
            }
        }
        public int Count
        {
            get { return interactionMessages.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            return interactionMessages.GetEnumerator();
        }
        #endregion
    }
}
