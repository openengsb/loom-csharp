using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Protocols;
using System.Collections;
using Protocols.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;

namespace TCPHandling
{
    /// <summary>
    /// New List, which overs the possibility to pars the elements
    /// </summary>
    class ETMList : IList
    {
        #region Variables
        private IDictionary<InteractionMessage, int> elements = new ConcurrentDictionary<InteractionMessage, int>();
        private IList<InteractionMessage> listelements = new List<InteractionMessage>();
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
            IProtocol protocol = element.protocol;
            if (!protocolTypes.Exists(pr => { return protocol.GetType().IsInstanceOfType(pr); }))
                protocolTypes.Add(protocol);
        }
        /// <summary>
        /// Search for an element and returns it when it could be found
        /// </summary>
        /// <param name="item">Search parameter</param>
        /// <param name="socketID">Socket, on which the message has been received</param>
        /// <returns>Found element</returns>
        public InteractionMessage getElement(IProtocol item,int socketID)
        {
            InteractionMessage canidates = null;
            int min = int.MaxValue;
            
            foreach (InteractionMessage message in listelements)
            {
                if (compaire(item, socketID, message))
                {
                    // When there are two messages in a configuration with the same type then
                    // the choosen message will be returned and then will be the last for the next serch
                    if (elements[message] < min)
                    {
                        canidates = message;
                        min = elements[message];
                    }
                }
            }
            if (canidates != null)
            {
                elements[canidates]++;
            }
            return canidates.Clone() as InteractionMessage;            
        }
        /// <summary>
        /// Compaires if the protocol is the same
        /// </summary>
        /// <param name="item">element1 to compaire</param>
        /// <param name="socketID">Socket number</param>
        /// <param name="message">element2 to compaire</param>
        /// <returns>Compaire result</returns>
        private bool compaire(IProtocol item, int socketID, InteractionMessage message)
        {
            return (message.protocol.SocketNumber == socketID || message.protocol.SocketNumber < 0) && message.protocol.Compaire(item);
        }
        /// <summary>
        /// Find a interaction in the list
        /// </summary>
        /// <param name="message">received message from a socket</param>
        /// <param name="SocketId">the socket number</param>
        /// <param name="convertedprotocol">Return value for the converted protocol</param>
        /// <returns>The result of the search</returns>
        public InteractionMessage findInteraction(byte[] message, int SocketId, out IProtocol convertedprotocol)
        {
            IProtocol receivedmessage;
            Boolean moreData = false;
            convertedprotocol = null;
            foreach (IProtocol protocol in protocolTypes)
            {
                receivedmessage = protocol.ConvertToProtocol(message);
                if (receivedmessage != null)
                {
                    if (!receivedmessage.getMoreBytes())
                    {
                        InteractionMessage canidat = getElement(receivedmessage, SocketId);
                        if (canidat != null)
                        {
                            convertedprotocol = receivedmessage;
                            return (InteractionMessage)canidat.Clone();
                        }
                    }
                    else
                        moreData = true;
                }
            }
            if (moreData)
                return null;
            throw new ArgumentException("It was unpossible, to find a configuration for the indicated format");
        }
        /// <summary>
        /// Adds a element to the list
        /// </summary>
        /// <param name="value">Element to add</param>
        /// <returns>Position of the added element</returns>
        public int Add(object value)
        {
            InteractionMessage element = (InteractionMessage)value;
            listelements.Add(element);
            elements.Add(element, 0);
            AddProtocolType(element);
            return listelements.Count-1;
        }
        /// <summary>
        /// Deletes all the elements
        /// </summary>
        public void Clear()
        {
            elements.Clear();
            listelements.Clear();
        }
        /// <summary>
        /// Checks if the list contains the element
        /// </summary>
        /// <param name="value">element to check</param>
        /// <returns>if it contains the element</returns>
        public bool Contains(object value)
        {
            InteractionMessage element = (InteractionMessage)value;
            return listelements.Contains(element);
        }
        /// <summary>
        /// returns Position of the element
        /// </summary>
        /// <param name="value">element to search</param>
        /// <returns>postion</returns>
        public int IndexOf(object value)
        {
            return listelements.IndexOf((InteractionMessage) value);
        }
        /// <summary>
        /// Insert a element
        /// </summary>
        /// <param name="index">position</param>
        /// <param name="value">elment</param>
        public void Insert(int index, object value)
        {
            InteractionMessage element = (InteractionMessage)value;
            elements.Add(element, 0);
            AddProtocolType(element);
            listelements.Insert(index, element);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return listelements.IsReadOnly; }
        }

        public void Remove(object value)
        {
            InteractionMessage element=value as InteractionMessage;
            listelements.Remove(element);
            elements.Remove(element);
        }

        public void RemoveAt(int index)
        {            
            elements.Remove(listelements[index]);
            listelements.RemoveAt(index);
        }

        public object this[int index]
        {
               get
            {                
                return listelements[index];
            }
            set
            {
                AddProtocolType((InteractionMessage)value);
                listelements[index] = (InteractionMessage) value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return listelements.Count; }
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
            return listelements.GetEnumerator();
        }
#endregion
    }
}
