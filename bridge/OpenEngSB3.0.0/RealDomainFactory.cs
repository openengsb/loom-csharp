#region Copyright
// <copyright file="RealDomainFactory.cs" company="OpenEngSB">
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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;
using Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote;

namespace Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300
{
    /// <summary>
    /// This class produces and manages proxies.
    /// </summary>
    public class RealDomainFactory<DomainReverseTyp> : AbstractRealDomainFactory<DomainReverseTyp>
    {
        #region Propreties

        public RealDomainFactory(string destination, DomainReverseTyp domainService, ABridgeExceptionHandling exceptionhandler)
            : base(destination, domainService, exceptionhandler)
        {
        }

        public RealDomainFactory(string destination, DomainReverseTyp domainService)
            : base(destination, domainService)
        {
        }

        public RealDomainFactory(string destination, DomainReverseTyp domainService, ABridgeExceptionHandling exceptionhandler, String username, String password)
            : base(destination, domainService, exceptionhandler, username, password)
        {
        }

        public RealDomainFactory(string destination, DomainReverseTyp domainService, String username, String password)
            : base(destination, domainService, username, password)
        {
        }

        public RealDomainFactory(string destination, DomainReverseTyp domainService, String contextId, ABridgeExceptionHandling exceptionhandler)
            : this(destination, domainService, exceptionhandler)
        {
            this.ContextId = contextId;
        }

        public RealDomainFactory(string destination, DomainReverseTyp domainService, String contextId)
            : this(destination, domainService)
        {
            this.ContextId = contextId;
        }

        public RealDomainFactory(string destination, DomainReverseTyp domainService, String contextId, ABridgeExceptionHandling exceptionhandler, String username, String password)
            : this(destination, domainService, exceptionhandler, username, password)
        {
            this.ContextId = contextId;
        }

        public RealDomainFactory(string destination, DomainReverseTyp domainService, String contextId, String username, String password)
            : this(destination, domainService, username, password)
        {
            this.ContextId = contextId;
        }

        #endregion

        #region Abstact Method Implementation

        protected override SubEventHandlerTyp GetSubEventhandler<SubEventHandlerTyp>(String connectorId)
        {
            return new DomainProxy<SubEventHandlerTyp>(Destination, connectorId, GetDomainType(connectorId), ContextId, Exceptionhandler).GetTransparentProxy();
        }

        protected override A GetSubEventhandler<A>(String connectorId, String username, String password)
        {
            return new DomainProxy<A>(Destination, connectorId, GetDomainType(connectorId), ContextId, Exceptionhandler, username, password).GetTransparentProxy();
        }

        protected override DomainReverse<DomainReverseTyp> CreateInstance(string serviceId, string domainName, String contextId, bool createConnector)
        {
            return new DomainReverseProxy<DomainReverseTyp>(DomainService, Destination, serviceId, domainName, contextId, createConnector, Exceptionhandler);
        }

        protected override DomainReverse<DomainReverseTyp> CreateInstance(string serviceId, string domainName, String contextId, bool createConnector, string username, string password)
        {
            return new DomainReverseProxy<DomainReverseTyp>(DomainService, Destination, serviceId, domainName, username, password, contextId, createConnector, Exceptionhandler);
        }
        #endregion
    }
}