#region Copyright
// <copyright file="JmsPort.cs" company="OpenEngSB">
// Licensed to the Austrian Association for Software Tool Integration (AASTI)
// under one or more contributor license agreements. See the NOTICE file
// distributed with this work for additional information regarding copyright
// ownership. The AASTI licenses this file to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms
{
    /// <summary>
    /// Abstract class to connect to a destination
    /// </summary>
    public abstract class JmsPort
    {
        #region Properties
        protected String ConnectorId
        {
            get;
            set;
        }

        protected IConnectionFactory Factory
        {
            get;
            set;
        }
        
        protected ABridgeExceptionHandling Handling
        {
            get;
            set;
        }
        #endregion
        #region Private Variables
        /// <summary>
        /// ActiveMQ NMS
        /// </summary>
        private static IDictionary<String, IDictionary<String, IConnection>> connections = new Dictionary<String, IDictionary<String, IConnection>>();

        private string stringDestination;
        #endregion
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="destination">Destionation to connect with OpenEngSB</param>
        protected JmsPort(string destination, ABridgeExceptionHandling handling, String connectorId)
        {
            this.ConnectorId = connectorId;
            this.Factory = null;
            this.stringDestination = destination;
            this.Handling = handling;
            Configure();
        }
        #endregion
        #region Properties
        protected IDestination Destination
        {
            get
            {
                JmsDestination dest = new JmsDestination(stringDestination);
                return Session.GetDestination(dest.Queue);
            }
        }

        protected ISession Session
        {
            get
            {
                return connections[ConnectorId][stringDestination].CreateSession();
            }
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Close the remaining connections
        /// </summary>
        public static void CloseAll(String connectorId)
        {
            foreach (IConnection connection in connections[connectorId].Values)
            {
                connection.Close();
                connection.Dispose();
            }

            connections[connectorId].Clear();
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        protected void Close()
        {
            if (connections[ConnectorId].ContainsKey(stringDestination))
            {
                connections[ConnectorId][stringDestination].Close();
                connections[ConnectorId][stringDestination].Dispose();
                connections[ConnectorId].Remove(stringDestination);
            }
        }

        /// <summary>
        /// Configurate the Connection
        /// </summary>
        /// <param name="destination">Destionation</param>
        protected void Configure()
        {
            if (DoesConnectionExists())
            {
                return;
            }

            try
            {
                JmsDestination dest = new JmsDestination(stringDestination);
                Uri connectionUri = new Uri(dest.Host);
                Factory = new Apache.NMS.ActiveMQ.ConnectionFactory(dest.Host);
                IConnection connection = Factory.CreateConnection();
                connection.Start();
                if (connections.ContainsKey(ConnectorId))
                {
                    connections[ConnectorId].Add(stringDestination, connection);
                }
                else
                {
                    IDictionary<String, IConnection> tmp = new Dictionary<String, IConnection>();
                    tmp.Add(stringDestination, connection);
                    connections.Add(ConnectorId, tmp);
                }
            }
            catch (Exception ex)
            {
                // This allows it to invoke the method "Listen" again
                // The exception handler (if configured) invokes Changed
                // that will be forwarded to delegate (Object[] notUsed)
                Handling.Changed += delegate(Object[] notUsed)
                {
                    Configure();
                    return null;
                };
                Handling.HandleException(ex);
            }
        }
        #endregion
        #region Private Methods
        private bool DoesConnectionExists()
        {
            return connections.ContainsKey(ConnectorId) && connections[ConnectorId].ContainsKey(stringDestination);
        }
        #endregion
    }
}