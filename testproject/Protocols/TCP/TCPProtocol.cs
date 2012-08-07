using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;

namespace Org.Openengsb.Loom.CSharp.Bridge.Protocol.Tcp
{
    /// <summary>
    /// Simulation of the TCP protocol
    /// </summary>
    public class TCPProtocol : IProtocol
    {
        #region Variables
        private byte[] message = null;
        public int SocketNumber { get; set; }
        #endregion
        #region Constructors
        public TCPProtocol() { this.SocketNumber = -1; }
        public TCPProtocol(byte[] message, int SocketID)
        {
            this.message = message;
            this.SocketNumber = SocketID;
        }
        #endregion
        #region Public methods
        public IProtocol ConvertToProtocol(Object message)
        {
            ASCIIEncoding asci = new ASCIIEncoding();
            return new TCPProtocol(asci.GetBytes((String)message), -1);
        }

        public IProtocol ConvertToProtocol(byte[] message)
        {
            return new TCPProtocol(message, -1);
        }
        public byte[] GetMessage() {
            return message; 
        }
        public int CompareTo(IProtocol protocol)
        {
            if (protocol is TCPProtocol)
            {
                TCPProtocol tcp = (TCPProtocol)protocol;
                if (CompaireByteArraysWithoutLength(tcp.GetMessage(), message))
                {
                    return 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// Compaires two arrays if they are equal.
        /// </summary>
        /// <param name="array1">The received message from the ETM</param>
        /// <param name="array2">The stored array</param>
        /// <returns></returns>
        private Boolean CompaireByteArraysWithoutLength(Byte[] array1, Byte[] array2)
        {
            if (array1 == null || array2 == null)
            {
                return false;
            }
            if (array1.Length > array2.Length)
            {
                Array.Resize(ref array2, array1.Length);
            }
            else if (array1.Length != array2.Length)
            {
                return false;
            }
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }
        public override string ToString()
        {
            ASCIIEncoding asci = new ASCIIEncoding();
            return asci.GetString(message);
        }

        public bool GetMoreBytes()
        {
            return message.Length < 10;
        }
        public void RetrieveInfoFromReceivedMessage(IProtocol receivedMessage)
        {
            return;
        }

        /// <summary>
        /// Create a copy of the protocol
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new TCPProtocol(GetMessage(), -1);
        }
        #endregion
    }
}