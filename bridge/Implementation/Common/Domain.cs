#region Copyright
// <copyright file="Domain.cs" company="OpenEngSB">
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
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using log4net;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.RemoteObjects;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    public abstract class Domain<TransparentProxyType> : RealProxy
    {
        #region Constants
        /// <summary>
        /// Name of the queue the server listens to for calls.
        /// </summary>
        public const string HostQueue = "receive";
        #endregion
        #region Properties
        public static ILog Logger
        {
            get;
            private set;
        }

        /// <summary>
        /// Authenifaction class
        /// </summary>
        public string AuthenificationClass
        {
            get;
            set;
        }

        /// <summary>
        /// Id identifying the service instance on the bus.
        /// </summary>
        public String ConnectorId
        {
            get;
            private set;
        }

        /// <summary>
        /// Domain type
        /// </summary>
        public String DomainName
        {
            get;
            private set;
        }

        public ABridgeExceptionHandling Exceptionhandler
        {
            get;
            private set;
        }

        /// <summary>
        /// Host string of the server.
        /// </summary>
        public string Host
        {
            get;
            private set;
        }

        public IMarshaller Marshaller
        {
            get;
            private set;
        }

        /// <summary>
        /// Password for the authentification
        /// </summary>
        public String Password
        {
            get;
            private set;
        }

        /// <summary>
        /// Username for the authentification
        /// </summary>
        public String Username
        {
            get;
            private set;
        }
        #endregion
        #region Constructors
        public Domain(string host, string connectorId, String domainName, ABridgeExceptionHandling exceptionhandler)
        : base(typeof(TransparentProxyType))
        {
            this.ConnectorId = connectorId;
            this.DomainName = domainName;
            this.Host = host;
            this.Marshaller = new JsonMarshaller();
            this.Username = "admin";
            this.Password = "password";
            this.Exceptionhandler = exceptionhandler;
            Logger = LogManager.GetLogger(typeof(TransparentProxyType));
        }

        public Domain(string host, string connectorId, String domainName, ABridgeExceptionHandling exceptionhandler, String username, String password)
        : this(host, connectorId, domainName, exceptionhandler)
        {
            this.Username = username;
            this.Password = password;
        }
        #endregion
        #region Public Methods
        public new TransparentProxyType GetTransparentProxy()
        {
            return (TransparentProxyType)base.GetTransparentProxy();
        }
        #endregion
        #region Protected Methods
        /// <summary>
        /// Builds an IMessage using MethodReturn.
        /// </summary>
        /// <param name="methodReturn">Servers return message</param>
        /// <param name="callMessage">Method an parameters</param>
        /// <returns>The result of the Message</returns>
        protected IMessage ToMessage(IMethodResult methodReturn, IMethodCallMessage callMessage)
        {
            Logger.Info("Convert method call to String method and send it to the OpenEngSB");
            switch (methodReturn.Type)
            {
            case ReturnType.Exception:
            {
                return new ReturnMessage(new BridgeException("Received an Excetion from the bridge", new OpenEngSBException(methodReturn.Arg.ToString(), new OpenEngSBException(methodReturn.ToString()))), callMessage);
            }

            case ReturnType.Void:
            case ReturnType.Object:
            {
                return new ReturnMessage(methodReturn.Arg, null, 0, null, callMessage);
            }

            default:
            {
                return null;
            }
            }
        }
        #endregion
    }
}