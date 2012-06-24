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
using Implementation.Common;
using Implementation.OpenEngSB2_4_0.Remote;
using Interface;

namespace Implementation.OpenEngSB2_4_0
{
    /// <summary>
    /// This class produces and manages proxies.
    /// </summary>
    public class RealDomainFactory : IDomainFactory
    {
        #region Variables
        private Dictionary<object, IRegistration> proxies;
        private String serviceId;
        private String domainType;        
        #endregion
        #region Attributes     
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public RealDomainFactory()
        {
            Reset();
        }
        #endregion
        #region Private Mehtods
        /// <summary>
        /// Remove all the proxies 
        /// </summary>
        private void Reset()
        {
            proxies = new Dictionary<object, IRegistration>();
        }
        #endregion
        #region Public Methods

        public bool Registered(object domainService)
        {
            return proxies[domainService].Registered;
        }
        /// <summary>
        /// Creates, registers and starts a reverse proxy with defaul Authentification
        /// RENAME
        /// </summary>
        /// <typeparam name="T">Type of remote Domain</typeparam>
        /// <param name="destination">Destination</param>
        /// <param name="domainService">Local Domain</param>
        /// <param name="domainType">Name of the Domain</param>
        public void CreateDomainService<T>(string destination, T domainService, String domainType)
        {
            this.domainType = domainType;
            this.serviceId = Guid.NewGuid().ToString();
            DomainReverseProxy<T> proxy = new DomainReverseProxy<T>(domainService, destination, serviceId, domainType);
            proxies.Add(domainService, proxy);
            proxy.Start();
        }
        /// <summary>
        /// Creates, registers and starts a reverse proxy with Authentification
        /// RENAME
        /// </summary>
        /// <typeparam name="T">Type of remote Domain</typeparam>
        /// <param name="destination">Destination</param>
        /// <param name="domainService">Local Domain</param>
        /// <param name="domainType">Name of the Domain</param>
        /// <param name="username">Username for the authentification</param>
        /// <param name="password">Password for the authentification</param>
        public void RegisterDomainService<T>(string destination, T domainService, String domainType, String username, String password)
        {
            this.domainType = domainType;
            this.serviceId = Guid.NewGuid().ToString();
            DomainReverseProxy<T> proxy = new DomainReverseProxy<T>(domainService, destination, serviceId, domainType, username, password);
            proxies.Add(domainService, proxy);
            proxy.Start();
        }
        /// <summary>
        /// Deletes and stops the reverse proxy.
        /// </summary>
        /// <param name="service">proxy to delete</param>
        public void DeleteDomainService(object service)
        {
            IRegistration stoppable = null;
            if(proxies.TryGetValue(service, out stoppable))
            {
                stoppable.Stop();
                proxies.Remove(service);
            }
        }
        /// <summary>
        /// Get Eventhandler from the spezified Domain T with authentification
        /// </summary>
        /// <typeparam name="T">Remote Domain</typeparam>
        /// <param name="host">Host</param>
        /// <param name="username">Username for the authentification</param>
        /// <param name="password">Password for the authentificaiton</param>
        /// <returns>Eventhandler</returns>
        public T getEventhandler<T>(string host,String username,String password)
        {
            return new DomainProxy<T>(host, getDomainTypServiceId(), domainType).GetTransparentProxy();
        }
        /// <summary>
        /// Get Eventhandler from the spezified Domain T
        /// </summary>
        /// <typeparam name="T">Remote Domain</typeparam>
        /// <param name="host">Host</param>
        /// <returns>Eventhandler</returns>
        public T getEventhandler<T>(string host)
        {
            return new DomainProxy<T>(host, getDomainTypServiceId(),domainType).GetTransparentProxy();
        }
        /// <summary>
        /// Returns the domainType + "+external-connector-proxy+" + serviceId
        /// </summary>
        /// <returns>String Value</returns>
        public String getDomainTypServiceId()
        {
            return domainType + "+external-connector-proxy+" + serviceId;
        }
        public String getServiceId()
        {
            return serviceId;
        }
        public void RegisterConnector<T>(string registerId, string destination, T service, string domainType)
        {
            throw new NotImplementedException();
        }

        public void UnRegisterConnector(object service)
        {
            throw new NotImplementedException();
        }
        #endregion



    }
}
