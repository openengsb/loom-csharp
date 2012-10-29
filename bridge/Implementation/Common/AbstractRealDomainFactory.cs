﻿/***
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
using ConnectorManager;
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
        private Dictionary<String, IRegistration> proxies;
        protected String destination;
        protected T domainService;
        protected ABridgeExceptionHandling exceptionhandler = new BridgeExceptionHandler();
        #endregion
        #region Constructors
        public AbstractRealDomainFactory(string destination, T domainService)
        {
            this.destination = destination;
            this.domainService = domainService;
            proxies = new Dictionary<String, IRegistration>();
            exceptionhandler = new BridgeExceptionHandler();
        }
        public AbstractRealDomainFactory(string destination, T domainService, ABridgeExceptionHandling exceptionhandler)
        {
            this.exceptionhandler = exceptionhandler;
            this.destination = destination;
            this.domainService = domainService;
            proxies = new Dictionary<String, IRegistration>();
        }
        #endregion
        #region Public Methods
        public bool Registered(String domainType)
        {
            if (proxies.ContainsKey(domainType))
                return proxies[domainType].Registered;
            return false;
        }
        /// <summary>
        /// Creates, registers and starts a reverse proxy.
        /// </summary>
        /// <typeparam name="T">local Domain Type</typeparam>
        /// <param name="destination">Registration destionation</param>
        /// <param name="domainService"></param>
        /// <param name="serviceId"></param>
        /// <param name="domainType">local domain</param>
        /// <param name="domainType">remote domain</param>
        /// <returns>ServiceID</returns>
        public String CreateDomainService(String domainType)
        {
            String serviceId = Guid.NewGuid().ToString();
            DomainReverse<T> proxy = createInstance(serviceId, domainType, true);
            proxies.Add(domainType, proxy);
            proxy.Start();
            return serviceId;
        }
        /// <summary>
        /// Creates, registers and starts a reverse proxy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="domainService"></param>
        /// <param name="serviceId"></param>
        /// <param name="domainType">local domain</param>
        /// <param name="domainType">remote domain</param>
        /// <param name="username">Username for the authentification</param>
        /// <param name="password">Password for the authentification</param>
        /// <returns>ServiceID</returns>
        public String CreateDomainService(String domainType, String username, String password)
        {
            String serviceId = Guid.NewGuid().ToString();
            DomainReverse<T> proxy = createInstance(serviceId, domainType, true, username, password);
            proxies.Add(domainType, proxy);
            proxy.Start();
            return serviceId;
        }
        /// <summary>
        /// Deletes and stops the reverse proxy.
        /// </summary>
        /// <param name="service"></param>
        public void DeleteDomainService(String domainType)
        {
            IRegistration stoppable = null;
            if (proxies.TryGetValue(domainType, out stoppable))
            {
                stoppable.DeleteRemoteProxy();
            }
        }
        /// <summary>
        /// returns the Domain+Txpe+ServiceId
        /// </summary>
        /// <returns></returns>
        public String getDomainTypServiceId(String domainType)
        {
            return domainType + "+external-connector-proxy+" + getServiceId(domainType);
        }
        /// <summary>
        /// return only the GUID of the service
        /// </summary>
        /// <returns>GUID from the service</returns>
        public String getServiceId(String domainType)
        {
            IRegistration stoppable = null;
            if (proxies.TryGetValue(domainType, out stoppable))
                return stoppable.ServiceID;

            return null;
        }
        public A getEventhandler<A>(String domainType)
        {
            return getSubEventhandler<A>(domainType);
        }
        /// <summary>
        /// Registers the connector again
        /// </summary>
        /// <typeparam name="T">Type of the domain</typeparam>
        /// <param name="registrationId">Id to register the connector</param>
        /// <param name="destination">URL to the OpenEngSB</param>
        /// <param name="domainService">Local implementation</param>
        /// <param name="domainType">Domain name</param>
        public void RegisterConnector(String serviceId, String domainType)
        {
            if (!proxies.ContainsKey(domainType))
            {
                DomainReverse<T> proxy = createInstance(serviceId, domainType, false);
                proxies.Add(domainType, proxy);
                proxy.Start();
            }
            else
            {
                proxies[domainType].RegisterConnector(serviceId);
            }
        }
        /// <summary>
        /// Unregisters a connector
        /// </summary>
        /// <param name="service">The service</param>
        public void UnRegisterConnector(String domainType)
        {
            IRegistration stoppable = null;
            if (proxies.TryGetValue(domainType, out stoppable))
            {
                stoppable.UnRegisterConnector();
            }
        }
        /// <summary>
        /// Closes the connection with the OpenEngSB
        /// </summary>
        /// <param name="service">Loacl implementaion</param>
        public void StopConnection(String domainType)
        {
            IRegistration stoppable = null;
            if (proxies.TryGetValue(domainType, out stoppable))
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
                proxies.Remove(domainType);
            }
        }
        public XLinkUrlBlueprint ConnectToXLink(string domainType, ModelToViewsTuple[] modelsToViews)
        {
            /*
             * Has to be discussed if Xlink can be without register
            if (!proxies.ContainsKey(domainType))
            {
                DomainReverse<T> proxy = createInstance( domainType, false);
                proxies.Add(domainType, proxy);
                proxy.Start();
            }*/
            return proxies[domainType].ConnectToXLink(domainType, modelsToViews);
        }
        public void DisconnectFromXLink(string domainType)
        {
            IRegistration stoppable = null;
            if (proxies.TryGetValue(domainType, out stoppable))
            {
                stoppable.DisconnectFromXLink();
            }
        }
        #endregion
        #region Abstract Methods
        /// <summary>
        /// Returns the DomainReverse object correct OpenEngSB version 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="domainName"></param>
        /// <param name="createConstructor"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>        
        protected abstract DomainReverse<T> createInstance(String serviceId, String domainType, Boolean createConstructor);
        /// <summary>
        /// Returns the DomainReverse object correct OpenEngSB version 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="domainName"></param>
        /// <param name="createConstructor"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>        
        protected abstract DomainReverse<T> createInstance(String serviceId, String domainName, Boolean createConstructor, String username, String password);
        /// <summary>
        /// Returns the eventhandler for the correct OpenEngSB version
        /// </summary>
        /// <typeparam name="A">Type of the Eventhandler</typeparam>
        /// <param name="domainType">DomainName</param>
        /// <returns>An eventHandler</returns>
        protected abstract A getSubEventhandler<A>(String domainName);
        #endregion

    }
}