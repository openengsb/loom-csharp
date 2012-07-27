﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Protocols.TCP
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
        public byte[] getMessage() {
            return message; 
        }
        public bool Compaire(Object protocol)
        {
            if (protocol is TCPProtocol)
            {
                TCPProtocol tcp = (TCPProtocol)protocol;
                return compaireObject(tcp.getMessage(), message);
            }
            return false;
        }
        private Boolean compaireObject(Byte[] array1, Byte[] array2)
        {

            if (array1 == null || array2 == null)
                return false;
            if (array1.Length > array2.Length)
            {
                Array.Resize(ref array2, array1.Length);
            }
            else if (array1.Length != array2.Length)
                return false;
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }
        public override string ToString()
        {
            ASCIIEncoding asci = new ASCIIEncoding();
            return asci.GetString(message);
        }

        public bool getMoreBytes()
        {
            return message.Length < 10;
        }
        public void RetrieveInfoFromReceivdeMessage(IProtocol receivedMessage)
        {
            return;
        }

        /// <summary>
        /// Create a copy of the protocol
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new TCPProtocol(getMessage(), -1);
        }
        #endregion
    }
}