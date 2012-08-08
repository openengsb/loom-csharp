using System;
using System.Net;
using Org.Openengsb.Loom.CSharp.Bridge.ETM;
using System.Collections.Generic;

namespace Org.Openengsb.Loom.CSharp.Bridge.Interfaces
{
    /// <summary>
    /// Interfaces description for the ETM
    /// </summary>
    public interface IETM : IDisposable
    {
        #region Method definition
        /// <summary>
        /// Start the ETM
        /// </summary>
        /// <param name="endpoint">Endpoint definition</param>
        void Start(IPEndPoint endpoint);
        /// <summary>
        /// Start the ETM
        /// </summary>
        /// <param name="adresse">IPAdresse</param>
        /// <param name="port">Port</param>
        void Start(IPAddress adresse, int port);
        /// <summary>
        /// Returns the received Messages, which the ETM has recorded
        /// </summary>
        IDictionary<int, Dictionary<int, IProtocol>> ReceivedMessages { get; }
        /// <summary>
        /// Sends a message to a client
        /// </summary>
        /// <param name="messagetoSend">Configuration</param>
        void TriggerMessage(InteractionMessage messagetoSend);
        /// <summary>
        /// Add an Interaction to the list
        /// </summary>
        /// <param name="interactionMessage">Interaction Message</param>
        void AddInteraction(InteractionMessage interactionMessage);
        #endregion
    }
}
