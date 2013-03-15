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
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.RemoteObjects;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using log4net;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    public abstract class Domain<T> : RealProxy
    {
        #region Const.
        /// <summary>
        /// Name of the queue the server listens to for calls.
        /// </summary>
        protected const string HOST_QUEUE = "receive";
        protected static ILog Logger;
        #endregion
        #region Variables
        protected ABridgeExceptionHandling Exceptionhandler;
        /// <summary>
        /// Authenifaction class
        /// </summary>
        protected string AUTHENTIFICATION_CLASS = "org.openengsb.core.api.security.model.UsernamePasswordAuthenticationInfo";
        /// <summary>
        /// Username for the authentification
        /// </summary>
        protected String Username;
        /// <summary>
        /// Password for the authentification
        /// </summary>
        protected String Password;
        /// <summary>
        /// Id identifying the service instance on the bus.
        /// </summary>
        protected String ConnectorId;
        /// <summary>
        /// Domain type
        /// </summary>
        protected String DomainName;

        /// <summary>
        /// Host string of the server.
        /// </summary>
        protected string Host;

        protected IMarshaller Marshaller;
        #endregion
        #region Constructors
        public Domain(string host, string connectorId, String domainName, ABridgeExceptionHandling exceptionhandler)
            : base(typeof(T))
        {
            this.ConnectorId = connectorId;
            this.DomainName = domainName;
            this.Host = host;
            this.Marshaller = new JsonMarshaller();
            this.Username = "admin";
            this.Password = "password";
            this.Exceptionhandler = exceptionhandler;
            Logger = LogManager.GetLogger(typeof(T));
        }
        public Domain(string host, string connectorId, String domainName, ABridgeExceptionHandling exceptionhandler, String username, String password)
            : this(host, connectorId, domainName, exceptionhandler)
        {
            this.Username = username;
            this.Password = password;
        }
        #endregion
        #region Public Methods
        public new T GetTransparentProxy()
        {
            return (T)base.GetTransparentProxy();
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
            switch (methodReturn.type)
            {
                case ReturnType.Exception:
                    {
                        return new ReturnMessage(new BridgeException("Received an Excetion from the bridge", new OpenEngSBException(methodReturn.arg.ToString(), new OpenEngSBException(methodReturn.ToString()))), callMessage);
                    }
                case ReturnType.Void:
                case ReturnType.Object:
                    {
                        return new ReturnMessage(methodReturn.arg, null, 0, null, callMessage);
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