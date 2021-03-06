﻿#region Copyright
// <copyright file="AbstractRealDomainFactory.cs" company="OpenEngSB">
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
using log4net;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    public abstract class AbstractRealDomainFactory<DomainImplementationType> : IDomainFactory
    {
        #region Properties
        public String Destination
        {
            get;
            private set;
        }

        public DomainImplementationType DomainService
        {
            get;
            private set;
        }

        public ABridgeExceptionHandling Exceptionhandler
        {
            get;
            private set;
        }

        public String ContextId
        {
            get;
            set;
        }

        #endregion
        #region Logger
        private static ILog logger = LogManager.GetLogger(typeof(AbstractRealDomainFactory<DomainImplementationType>));
        #endregion
        #region Private Variables
        private String password;
        private String username;
        private Boolean defaultUsernameAndPassword = true;
        private Dictionary<String, IRegistration> proxies;
        #endregion
        #region Constructors
        public AbstractRealDomainFactory()
        {
            Exceptionhandler = new RetryDefaultExceptionHandler();
            proxies = new Dictionary<String, IRegistration>();
            Exceptionhandler = new RetryDefaultExceptionHandler();
            defaultUsernameAndPassword = true;
        }

        public AbstractRealDomainFactory(string destination, DomainImplementationType domainService)
            : this()
        {
            this.Destination = destination;
            this.DomainService = domainService;
        }

        public AbstractRealDomainFactory(string destination, DomainImplementationType domainService, String username, String password)
            : this(destination, domainService)
        {
            this.username = username;
            this.password = password;
            defaultUsernameAndPassword = false;
        }

        public AbstractRealDomainFactory(string destination, DomainImplementationType domainService, ABridgeExceptionHandling exceptionhandler)
            : this(destination, domainService)
        {
            this.Exceptionhandler = exceptionhandler;
            this.Destination = destination;
            this.DomainService = domainService;
            proxies = new Dictionary<String, IRegistration>();
            defaultUsernameAndPassword = true;
        }

        public AbstractRealDomainFactory(string destination, DomainImplementationType domainService, ABridgeExceptionHandling exceptionhandler, String username, String password)
            : this(destination, domainService, exceptionhandler)
        {
            this.username = username;
            this.password = password;
            defaultUsernameAndPassword = false;
        }

        #endregion
        #region Public Methods
        public XLinkUrlBlueprint ConnectToXLink(string connectorId, String hostId, String domainName, ModelToViewsTuple[] modelsToViews)
        {
            IRegistration connector = null;
            if (proxies.TryGetValue(connectorId, out connector))
            {
                if (connector.Registered)
                {
                    return connector.ConnectToXLink(domainName, hostId, modelsToViews);
                }
            }

            throw new BridgeException("The connecotr with id " + connectorId + " has no instance");
        }

        /// <summary>
        /// Creates, registers and starts a reverse proxy.
        /// </summary>
        /// <typeparam name="T">local Domain Type</typeparam>
        /// <param name="destination">Registration destionation</param>
        /// <param name="domainService"></param>
        /// <param name="ConnectorId"></param>
        /// <param name="domainName">local domain</param>
        /// <param name="domainName">remote domain</param>
        /// <returns>ConnectorId</returns>
        public String CreateDomainService(String domainName)
        {
            return CreateDomainServiceOrReregisterExisting(Guid.NewGuid().ToString(), domainName, true);
        }

        /// <summary>
        /// Deletes and stops the reverse proxy.
        /// </summary>
        /// <param name="service"></param>
        public void DeleteDomainService(String connectorId)
        {
            IRegistration stoppable = null;
            if (proxies.TryGetValue(connectorId, out stoppable))
            {
                stoppable.DeleteRemoteProxy();
            }
        }

        public void DisconnectFromXLink(string connectorId, String hostId)
        {
            IRegistration stoppable = null;
            if (proxies.TryGetValue(connectorId, out stoppable))
            {
                stoppable.DisconnectFromXLink(hostId);
            }
        }

        /// <summary>
        /// returns the Domain+Txpe+ConnectorId
        /// </summary>
        /// <returns></returns>
        public String GetDomainTypConnectorId(String connectorId)
        {
            String domainType = GetDomainType(connectorId);
            if (String.IsNullOrEmpty(domainType))
            {
                throw new BridgeException("There is no connector with the connectorId " + connectorId);
            }

            return domainType + "+external-connector-proxy+" + connectorId;
        }

        /// <summary>
        /// Returns the DomainType of a connector
        /// </summary>
        /// <param name="connectorId">Guid od the connector</param>
        /// <returns></returns>
        public String GetDomainType(String connectorId)
        {
            IRegistration connector = null;
            if (proxies.TryGetValue(connectorId, out connector))
            {
                return connector.DomainName;
            }

            return null;
        }

        public InterfaceDomainType GetEventhandler<InterfaceDomainType>(String connectorId)
        {
            if (defaultUsernameAndPassword)
            {
                return GetSubEventhandler<InterfaceDomainType>(connectorId);
            }
            else
            {
                return GetSubEventhandler<InterfaceDomainType>(connectorId, username, password);
            }
        }

        /// <summary>
        /// Registers the connector again
        /// </summary>
        /// <typeparam name="T">Type of the domain</typeparam>
        /// <param name="registrationId">Id to register the connector</param>
        /// <param name="destination">URL to the OpenEngSB</param>
        /// <param name="domainService">Local implementation</param>
        /// <param name="domainName">Domain name</param>
        public String RegisterConnector(String connectorId, String domainName)
        {
            String uuid = connectorId;
            if (!String.IsNullOrEmpty(uuid) && !proxies.ContainsKey(uuid))
            {
                try
                {
                    uuid = CreateDomainServiceOrReregisterExisting(connectorId, domainName, false);
                    return uuid;
                }
                catch (OpenEngSBException)
                {
                    uuid = CheckIfConnectorExistAndCreateConenctorIfNot(domainName, uuid);
                }
            }
            else
            {
                uuid = CheckIfConnectorExistAndCreateConenctorIfNot(domainName, uuid);
            }

            proxies[uuid].RegisterConnector(uuid);
            return uuid;
        }

        public bool Registered(String connectorId)
        {
            if (proxies.ContainsKey(connectorId))
            {
                return proxies[connectorId].Registered;
            }

            return false;
        }

        public void StopAllConnections()
        {
            Exceptionhandler.Stop = true;
            foreach (IRegistration stoppable in proxies.Values)
            {
                stoppable.Stop();
            }

            proxies.Clear();
        }

        /// <summary>
        /// Closes the connection with the OpenEngSB
        /// </summary>
        /// <param name="service">Loacl implementaion</param>
        public void StopConnection(String connectorId)
        {
            IRegistration stoppable = null;
            if (proxies.TryGetValue(connectorId, out stoppable))
            {
                stoppable.UnRegisterConnector();
                stoppable.Stop();
                proxies.Remove(connectorId);
            }
        }

        /// <summary>
        /// Unregisters a connector
        /// </summary>
        /// <param name="service">The service</param>
        public void UnRegisterConnector(String connectorId)
        {
            IRegistration stoppable = null;
            if (proxies.TryGetValue(connectorId, out stoppable))
            {
                stoppable.UnRegisterConnector();
            }
        }
        #endregion
        #region Abstract Methods
        /// <summary>
        /// Returns the DomainReverse object correct OpenEngSB version
        /// </summary>
        /// <param name="connectorId"></param>
        /// <param name="domainName"></param>
        /// <param name="createConstructor"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected abstract DomainReverse<DomainImplementationType> CreateInstance(String connectorId, String domainName, String contextId, Boolean createConstructor);

        /// <summary>
        /// Returns the DomainReverse object correct OpenEngSB version
        /// </summary>
        /// <param name="connectorId"></param>
        /// <param name="domainName"></param>
        /// <param name="createConstructor"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected abstract DomainReverse<DomainImplementationType> CreateInstance(String connectorId, String domainName, String contextId, Boolean createConstructor, String username, String password);

        /// <summary>
        /// Returns the eventhandler for the correct OpenEngSB version
        /// </summary>
        /// <typeparam name="InterfaceTyp">Type of the Eventhandler</typeparam>
        /// <param name="domainType">DomainName</param>
        /// <returns>An eventHandler</returns>
        protected abstract InterfaceTyp GetSubEventhandler<InterfaceTyp>(String connectorId);

        /// <summary>
        /// Returns the eventhandler for the correct OpenEngSB version
        /// </summary>
        /// <typeparam name="InterfaceTyp">Type of the Eventhandler</typeparam>
        /// <param name="connectorId">ConnectorId</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>An eventHandler</returns>
        protected abstract InterfaceTyp GetSubEventhandler<InterfaceTyp>(String connectorId, String username, String password);
        #endregion
        #region Private Methods
        private string CheckIfConnectorExistAndCreateConenctorIfNot(String domainName, String uuid)
        {
            if (String.IsNullOrEmpty(uuid) || !proxies.ContainsKey(uuid))
            {
                uuid = CreateDomainService(domainName);
            }

            return uuid;
        }

        /// <summary>
        /// Create or register a connector (depending on createNew (true=Create; false=Register)
        /// </summary>
        /// <param name="domainName">Domain Name</param>
        /// <param name="createNew">Create new connector</param>
        /// <returns>uuid</returns>
        private string CreateDomainServiceOrReregisterExisting(String connectorId, String domainName, Boolean createNew)
        {
            DomainReverse<DomainImplementationType> proxy;
            if (defaultUsernameAndPassword)
            {
                proxy = CreateInstance(connectorId, domainName, ContextId, createNew);
            }
            else
            {
                proxy = CreateInstance(connectorId, domainName, ContextId, createNew, username, password);
            }

            proxies.Add(connectorId, proxy);
            try
            {
                proxy.Start();
            }
            catch (OpenEngSBException ex)
            {
                proxy.Stop();
                proxies.Remove(connectorId);
                throw ex;
            }

            return connectorId;
        }
        #endregion
    }
}