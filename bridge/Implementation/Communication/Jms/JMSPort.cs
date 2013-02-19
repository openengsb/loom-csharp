using System;
/***
 * Licensed to the Austrian Association for Software Tool Integration (AASTI)
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. The AASTI licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 ***/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.NMS;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;
using Apache.NMS.ActiveMQ;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms
{
    /// <summary>
    /// Abstract class to connect to a destination
    /// </summary>
    public abstract class JmsPort
    {
        #region Variables
        /// <summary>
        /// ActiveMQ NMS
        /// </summary>
        private static IDictionary<String, IConnection> connections = new Dictionary<String, IConnection>();
        protected IConnectionFactory factory;
        protected ISession session
        {
            get
            {
                return connections[string_destination].CreateSession();
            }
        }
        protected IDestination destination
        {
            get
            {
                Destination dest = new Destination(string_destination);
                return session.GetDestination(dest.Queue);
            }
        }
        private string string_destination;
        protected ABridgeExceptionHandling handling;
        protected Boolean close = false;
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="destination">Destionation to connect with OpenEngSB</param>
        protected JmsPort(string destination, ABridgeExceptionHandling handling)
        {
            this.factory = null;
            this.string_destination = destination;
            this.handling = handling;
            Configure();
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Configurate the Connection
        /// </summary>
        /// <param name="destination">Destionation</param>
        protected void Configure()
        {
            if (close || connections.ContainsKey(string_destination))
            {
                return;
            }

            try
            {
                Destination dest = new Destination(string_destination);
                Uri connectionUri = new Uri(dest.Host);
                factory = new Apache.NMS.ActiveMQ.ConnectionFactory(dest.Host);
                IConnection connection = factory.CreateConnection();
                connection.ConnectionInterruptedListener += () =>
                {
                    throw new BridgeException("Connection has been interrupted");
                };
                connection.ExceptionListener += e =>
                {
                    throw new BridgeException("Conenction has thrown the exception", e);
                };

                connection.Start();

                connections.Add(string_destination, connection);
            }
            catch (Exception ex)
            {
                handling.Changed += delegate(Object[] asd)
                {
                    Configure();
                    return null;
                };
                handling.HandleException(ex);
            }
        }
        #endregion
        #region Protected Methods
        /// <summary>
        /// Close the connection
        /// </summary>
        protected void Close()
        {
            if (connections.ContainsKey(string_destination))
            {
                connections[string_destination].Close();
                connections[string_destination].Dispose();
                connections.Remove(string_destination);
            }
            close = true;
        }
        /// <summary>
        /// Close the remaining connections
        /// </summary>
        public static void CloseAll()
        {
            foreach (IConnection connection in connections.Values)
            {
                connection.Close();
                connection.Dispose();
            }
        }
        #endregion
    }
}