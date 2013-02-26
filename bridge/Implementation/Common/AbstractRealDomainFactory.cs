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
using System;
using System.Collections.Generic;
using OpenEngSBCore;
using log4net;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;


namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    public abstract class AbstractRealDomainFactory<T> : IDomainFactory
    {
        private static ILog logger = LogManager.GetLogger(typeof(AbstractRealDomainFactory<T>));

        #region Variables
        private Dictionary<String, IRegistration> Proxies;
        protected String Destination;
        protected T DomainService;
        protected ABridgeExceptionHandling Exceptionhandler = new RetryDefaultExceptionHandler();
        #endregion
        #region Constructors
        public AbstractRealDomainFactory(string destination, T domainService)
        {
            this.Destination = destination;
            this.DomainService = domainService;
            Proxies = new Dictionary<String, IRegistration>();
            Exceptionhandler = new RetryDefaultExceptionHandler();
        }
        public AbstractRealDomainFactory(string destination, T domainService, ABridgeExceptionHandling exceptionhandler)
        {
            this.Exceptionhandler = exceptionhandler;
            this.Destination = destination;
            this.DomainService = domainService;
            Proxies = new Dictionary<String, IRegistration>();
        }
        #endregion
        #region Public Methods
        public bool Registered(String connectorId)
        {
            if (Proxies.ContainsKey(connectorId))
            {
                return Proxies[connectorId].Registered;
            }
            return false;
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
            String connectorId = Guid.NewGuid().ToString();
            DomainReverse<T> proxy = CreateInstance(connectorId, domainName, true);
            Proxies.Add(connectorId, proxy);
            proxy.Start();
            return connectorId;
        }
        /// <summary>
        /// Creates, registers and starts a reverse proxy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="domainService"></param>
        /// <param name="ConnectorId"></param>
        /// <param name="domainName">local domain</param>
        /// <param name="domainName">remote domain</param>
        /// <param name="username">Username for the authentification</param>
        /// <param name="password">Password for the authentification</param>
        /// <returns>ConnectorId</returns>
        public String CreateDomainService(String domainName, String username, String password)
        {
            String connectorId = Guid.NewGuid().ToString();
            DomainReverse<T> proxy = CreateInstance(connectorId, domainName, true, username, password);
            Proxies.Add(domainName, proxy);
            proxy.Start();
            return connectorId;
        }
        /// <summary>
        /// Deletes and stops the reverse proxy.
        /// </summary>
        /// <param name="service"></param>
        public void DeleteDomainService(String connectorId)
        {
            IRegistration stoppable = null;
            if (Proxies.TryGetValue(connectorId, out stoppable))
            {
                stoppable.DeleteRemoteProxy();
            }
        }
        /// <summary>
        /// returns the Domain+Txpe+ConnectorId
        /// </summary>
        /// <returns></returns>
        public String GetDomainTypConnectorId(String connectorId)
        {
            return GetDomainType(connectorId) + "+external-connector-proxy+" + GetConnectorId(connectorId);
        }
        /// <summary>
        /// Returns the DomainType of a connector
        /// </summary>
        /// <param name="connectorId">Guid od the connector</param>
        /// <returns></returns>
        public String GetDomainType(String connectorId)
        {
            IRegistration connector = null;
            if (Proxies.TryGetValue(connectorId, out connector))
            {
                return connector.DomainName;
            }
            return null;            
        }
        /// <summary>
        /// return only the GUID of the service
        /// </summary>
        /// <returns>GUID from the service</returns>
        public String GetConnectorId(String connectorId)
        {
            IRegistration stoppable = null;
            if (Proxies.TryGetValue(connectorId, out stoppable))
            {
                return stoppable.ConnectorId;
            }
            return null;
        }
        public A GetEventhandler<A>(String connectorId)
        {
            return GetSubEventhandler<A>(connectorId);
        }
        /// <summary>
        /// Registers the connector again
        /// </summary>
        /// <typeparam name="T">Type of the domain</typeparam>
        /// <param name="registrationId">Id to register the connector</param>
        /// <param name="destination">URL to the OpenEngSB</param>
        /// <param name="domainService">Local implementation</param>
        /// <param name="domainName">Domain name</param>
        public void RegisterConnector(String connectorId, String domainName)
        {
            if (!Proxies.ContainsKey(connectorId))
            {
                CreateDomainService(domainName);
            }
            Proxies[connectorId].RegisterConnector(connectorId);
        }
        /// <summary>
        /// Unregisters a connector
        /// </summary>
        /// <param name="service">The service</param>
        public void UnRegisterConnector(String connectorId)
        {
            IRegistration stoppable = null;
            if (Proxies.TryGetValue(connectorId, out stoppable))
            {
                stoppable.UnRegisterConnector();
            }
        }
        /// <summary>
        /// Closes the connection with the OpenEngSB
        /// </summary>
        /// <param name="service">Loacl implementaion</param>
        public void StopConnection(String connectorId)
        {
            IRegistration stoppable = null;
            if (Proxies.TryGetValue(connectorId, out stoppable))
            {
                try
                {
                    stoppable.UnRegisterConnector();
                    stoppable.Stop();
                }
                catch (BridgeException e)
                {
                    logger.Warn("could not unregister. Maybe it is already unregistered " + stoppable);
                    logger.Info("ExceptionMessage: " + e.Message);
                }
                catch (OpenEngSBException e)
                {
                    logger.Warn("could not unregister. Maybe it is already unregistered " + stoppable);
                    logger.Info("ExceptionMessage: " + e.Message);
                }
                catch (Exception e)
                {
                    logger.Error("Unexpacted exception while unregistering connector" + stoppable, e);
                }
                Proxies.Remove(connectorId);
            }
        }
        public XLinkUrlBlueprint ConnectToXLink(string connectorId, String hostId, String domainName, ModelToViewsTuple[] modelsToViews)
        {
            /*
             * Has to be discussed if Xlink can be without register
            if (!proxies.ContainsKey(domainType))
            {
                DomainReverse<T> proxy = createInstance( domainType, false);
                proxies.Add(domainType, proxy);
                proxy.Start();
            }*/
            return Proxies[connectorId].ConnectToXLink(domainName, hostId, modelsToViews);
        }
        public void DisconnectFromXLink(string connectorId, String hostId)
        {
            IRegistration stoppable = null;
            if (Proxies.TryGetValue(connectorId, out stoppable))
            {
                stoppable.DisconnectFromXLink(hostId);
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
        protected abstract DomainReverse<T> CreateInstance(String connectorId, String domainName, Boolean createConstructor);
        /// <summary>
        /// Returns the DomainReverse object correct OpenEngSB version 
        /// </summary>
        /// <param name="connectorId"></param>
        /// <param name="domainName"></param>
        /// <param name="createConstructor"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>        
        protected abstract DomainReverse<T> CreateInstance(String connectorId, String domainName, Boolean createConstructor, String username, String password);
        /// <summary>
        /// Returns the eventhandler for the correct OpenEngSB version
        /// </summary>
        /// <typeparam name="A">Type of the Eventhandler</typeparam>
        /// <param name="domainType">DomainName</param>
        /// <returns>An eventHandler</returns>
        protected abstract A GetSubEventhandler<A>(String connectorId);
        #endregion

    }
}