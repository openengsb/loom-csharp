using System;
using System.Collections.Generic;
using System.Net;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;

namespace Org.Openengsb.Loom.CSharp.Bridge.ETM
{
    /// <summary>
    /// Devition of the interaction betwwen a component and the ETM.
    /// The devition inculdes the Endpoint (IP and Port) on which the components send the messages,
    /// the send message and a list of response, which should be sent as answer.
    /// </summary>
    public class InteractionMessage : IComparable, ICloneable
    {
        #region Variables
        public int? SourcePort { get; set; }
        public int? DestinationPort { get; set; }
        public IProtocol Protocol { get; set; }
        public long Timestamp { get; set; }
        public IPAddress SourceIPAddress { get; set; }
        public IPAddress DestinationIPAddress { get; set; }
        public List<InteractionMessage> Responses { get; set; }
        #endregion
        #region Constructors
        public InteractionMessage(int? source, int? dest, IProtocol protocol, List<InteractionMessage> response)
        {
            this.SourcePort = source;
            this.DestinationPort = dest;
            this.Protocol = protocol;
            this.Timestamp = DateTime.Now.Ticks;
            this.DestinationIPAddress = IPAddress.Loopback;
            this.SourceIPAddress = IPAddress.Loopback;
            this.Responses = response;
        }
        public InteractionMessage(int? source, int? dest, IProtocol protocol, List<InteractionMessage> response, IPAddress sourcedresse, IPAddress destination)
            : this(source, dest, protocol, response)
        {
            this.DestinationIPAddress = destination;
            this.SourceIPAddress = sourcedresse;
        }

        #endregion
        #region Public Methods
        /// <summary>
        /// Compaires to TransactionMessages
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return -Math.Sign(((InteractionMessage)obj).Timestamp - Timestamp);
        }
        /// <summary>
        /// Creates a copy
        /// </summary>
        /// <returns>Copy of this instance</returns>
        public object Clone()
        {
            List<InteractionMessage> result = new List<InteractionMessage>();
            if (Responses != null)
            {
                foreach (InteractionMessage tr in Responses)
                {
                    result.Add((InteractionMessage)tr.Clone());
                }
            }
            return new InteractionMessage(SourcePort, DestinationPort, (IProtocol)Protocol.Clone(), result, SourceIPAddress, DestinationIPAddress);
        }
        #endregion
    }
}