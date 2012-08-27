using System;

namespace Org.Openengsb.Loom.CSharp.Bridge.Interfaces
{
    public interface IProtocol : ICloneable,IComparable<IProtocol>
    {
        /// <summary>
        /// The id of the socket on which the message should be
        /// transmitted or on which it has been received
        /// </summary>
        int SocketNumber { get; set; }
        /// <summary>
        /// Convert a byte message to a the indicated Protocol
        /// </summary>
        /// <param name="message">Message to convert in bytes</param>
        /// <returns>IProtocol</returns>
        IProtocol ConvertToProtocol(Byte[] message);
        /// <summary>
        /// Offers the protocol the possibility to get information from the received message.
        /// </summary>
        /// <param name="receivedMessage">Received Message</param>
        void RetrieveInfoFromReceivedMessage(IProtocol receivedMessage);
        /// <summary>
        /// Converts the message from the protocol to a byte array
        /// </summary>
        /// <returns>protocol message in byte format</returns>
        Byte[] GetMessage();
        /// <summary>
        /// Indicates if a a protocol has received all bytes or if some bytes aren't still transmitted
        /// </summary>
        /// <returns>Need more bytes</returns>
        Boolean Valid();
    }
}