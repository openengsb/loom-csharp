﻿using System;
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
        protected IConnection connection;
        protected IConnectionFactory factory;
        protected ISession session;
        protected IDestination destination;
        private string string_destination;
        private EExceptionHandling handling;
        private int nbrretry;
        private int maxretries=int.MaxValue;
        protected Boolean close = false;
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="destination">Destionation to connect with OpenEngSB</param>
        protected JmsPort(string destination, EExceptionHandling handling)
        {
            this.connection = null;
            this.factory = null;
            this.session = null;
            this.destination = null;
            this.string_destination=destination;
            this.handling=handling;
            this.nbrretry = 0;
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
            if (close) return;
            if (nbrretry++ >= maxretries)
                handling = EExceptionHandling.ForwardException;
            
            try
            {
                Destination dest = new Destination(string_destination);
                Uri connectionUri = new Uri(dest.Host);
                factory = new Apache.NMS.ActiveMQ.ConnectionFactory(connectionUri);
                connection = factory.CreateConnection();
                session = connection.CreateSession();
                connection.Start();
                this.destination = session.GetDestination(dest.Queue);
                nbrretry = 0;
            }
            catch
            {
                switch (handling)
                {
                    case EExceptionHandling.ForwardException:
                        {
                            throw;
                        }
                    case EExceptionHandling.Retry:
                        {
                            Configure();
                            break;
                        }
                }
            }
        }
        #endregion
        #region Protected Methods
        /// <summary>
        /// Close the connection
        /// </summary>
        protected void Close()
        {
            connection.Close();
            connection = null;
            close = true;
        }
        #endregion
    }
}