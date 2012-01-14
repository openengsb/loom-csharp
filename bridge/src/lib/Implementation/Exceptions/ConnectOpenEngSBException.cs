using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Exceptions
{
    /// <summary>
    /// Exception for Problems with the OpenEngSB Connection
    /// </summary>
    public class ConnectOpenEngSBException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ConnectOpenEngSBException() { }
        /// <summary>
        /// Consructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public ConnectOpenEngSBException(string message) : base(message) { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception Message</param>
        /// <param name="inner">Inner Exception</param>
        public ConnectOpenEngSBException(string message, Exception inner)
            : base(message, inner) { }
    }
}
