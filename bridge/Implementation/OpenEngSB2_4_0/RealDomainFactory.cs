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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB2_4_0.Remote;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB2_4_0
{
    public class RealDomainFactory<T> : AbstractRealDomainFactory<T>
    {
        #region Constructor
        public RealDomainFactory(string destination, T domainService, ABridgeExceptionHandling exceptionhandler)
            : base(destination, domainService, exceptionhandler)
        {
        }
        public RealDomainFactory(string destination, T domainService)
            : base(destination, domainService)
        {
        }
        public RealDomainFactory(string destination, T domainService, ABridgeExceptionHandling exceptionhandler, string username, string password)
            : base(destination, domainService, exceptionhandler, username, password)
        {
        }
        public RealDomainFactory(string destination, T domainService, string username, string password)
            : base(destination, domainService, username, password)
        {
        }
        #endregion
        #region Abstact Method Implementation
        protected override A GetSubEventhandler<A>(String connectorId, String username, String password)
        {
            return new DomainProxy<A>(Destination, GetDomainTypConnectorId(connectorId), GetDomainType(connectorId), Exceptionhandler, username, password).GetTransparentProxy();
        }
        protected override A GetSubEventhandler<A>(String connectorId)
        {
            return new DomainProxy<A>(Destination, GetDomainTypConnectorId(connectorId), GetDomainType(connectorId), Exceptionhandler).GetTransparentProxy();
        }
        protected override DomainReverse<T> CreateInstance(string connectorId, string domainName, bool createConnector)
        {
            return new DomainReverseProxy<T>(DomainService, Destination, connectorId, domainName, Exceptionhandler);
        }
        protected override DomainReverse<T> CreateInstance(string serviceId, string domainType, bool createConnector, string username, string password)
        {
            return new DomainReverseProxy<T>(DomainService, Destination, serviceId, domainType, username, password, Exceptionhandler);
        }
        #endregion
    }
}