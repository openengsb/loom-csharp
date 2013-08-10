#region Copyright
// <copyright file="DomainProxy.cs" company="OpenEngSB">
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Xml.Serialization;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;
using Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects;

namespace Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote
{
    /// <summary>
    /// This class generates generic proxies. All method calls will be forwared to the configured server.
    /// </summary>
    /// <typeparam name="ProxyTyp">Typ to proxy.</typeparam>
    public class DomainProxy<ProxyTyp> : Domain<ProxyTyp>
    {
        #region Constructors

        public DomainProxy(string host, string connectorId, String domainName, ABridgeExceptionHandling exceptionhandler)
            : base(host, connectorId, domainName, exceptionhandler)
        {
            AuthenificationClass = "org.openengsb.connector.usernamepassword.Password";
        }

        public DomainProxy(string host, string connectorId, String domainName, ABridgeExceptionHandling exceptionhandler, String username, String password)
            : base(host, connectorId, domainName, exceptionhandler, username, password)
        {
            AuthenificationClass = "org.openengsb.connector.usernamepassword.Password";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Will be invoked when a call to the proxy has been made.
        /// </summary>
        /// <param name="msg">Message, which contains the Parameters of the Method</param>
        /// <returns>Received Method</returns>
        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage callMessage = msg as IMethodCallMessage;
            MethodCallMessage methodCallRequest = ToMethodCallRequest(callMessage);
            string methodCallMsg = Marshaller.MarshallObject(methodCallRequest);
            IOutgoingPort portOut = new JmsOutgoingPort(JmsDestination.CreateDestinationString(Host, HostQueue), Exceptionhandler, ConnectorId);
            portOut.Send(methodCallMsg, methodCallRequest.CallId);
            IIncomingPort portIn = new JmsIncomingPort(JmsDestination.CreateDestinationString(Host, methodCallRequest.CallId), Exceptionhandler, ConnectorId);
            string methodReturnMsg = portIn.Receive();
            MethodResultMessage methodReturn = Marshaller.UnmarshallObject<MethodResultMessage>(methodReturnMsg);
            return ToMessage(methodReturn.Result, callMessage);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Builds an MethodCall using IMethodCallMessage.
        /// </summary>
        /// <param name="msg">Information, to create a MethodCallRequest</param>
        /// <returns>A new instance of methodCallrequest</returns>
        private MethodCallMessage ToMethodCallRequest(IMethodCallMessage msg)
        {
            Guid id = Guid.NewGuid();

            string methodName = msg.MethodName;
            Dictionary<string, string> metaData = new Dictionary<string, string>();
            
            // The structure is always domain.DOMAINTYPE.events
            metaData.Add("serviceId", "domain." + DomainName + ".events");
            
            // Arbitrary string, maybe not necessary
            metaData.Add("contextId", "foo");
            List<string> classes = new List<string>();
            foreach (object arg in msg.Args)
            {
                if (arg != null)
                {
                    LocalType type = new LocalType(arg.GetType());
                    classes.Add(type.RemoteTypeFullName);
                }
                else
                {
                    classes.Add(null);
                }
            }

            RemoteMethodCall call = RemoteMethodCall.CreateInstance(methodName, msg.Args, metaData, classes, null);
            BeanDescription authentification = BeanDescription.CreateInstance(AuthenificationClass);
            authentification.Data.Add("value", Password);
            MethodCallMessage message = MethodCallMessage.CreateInstance(Username, authentification, call, id.ToString(), true, String.Empty);
            return message;
        }

        #endregion
    }
}