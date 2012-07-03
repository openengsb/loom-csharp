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
using System.Runtime.Remoting.Messaging;
using Implementation.Common;
using Implementation.Communication;
using Implementation.Communication.Jms;
using Implementation.OpenEngSB2_4_0.Remote.RemoteObjects;
namespace Implementation.OpenEngSB2_4_0.Remote
{
    /// <summary>
    /// This class generates generic proxies. All method calls will be forwared to the configured server.
    /// </summary>
    /// <typeparam name="T">Type to proxy.</typeparam>
    public class DomainProxy<T> : Domain<T>
    {
        #region Constructors
        public DomainProxy(string host, string serviceId, String domainType)
            : base(host, serviceId, domainType)
        {
            AUTHENTIFICATION_CLASS = "org.openengsb.core.api.security.model.UsernamePasswordAuthenticationInfo";
        }
        public DomainProxy(string host, string serviceId, String domainType, String username, String password)
            : base(host, serviceId, domainType, username, password)
        {
            AUTHENTIFICATION_CLASS = "org.openengsb.core.api.security.model.UsernamePasswordAuthenticationInfo";
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
            MethodCallRequest methodCallRequest = ToMethodCallRequest(callMessage);
            string methodCallMsg = marshaller.MarshallObject(methodCallRequest);
            IOutgoingPort portOut = new JmsOutgoingPort(Destination.CreateDestinationString(host, HOST_QUEUE));
            portOut.Send(methodCallMsg);
            IIncomingPort portIn = new JmsIncomingPort(Destination.CreateDestinationString(host, methodCallRequest.message.callId));
            string methodReturnMsg = portIn.Receive();
            MethodResultMessage methodReturn = marshaller.UnmarshallObject<MethodResultMessage>(methodReturnMsg);
            return ToMessage(methodReturn.message.result, callMessage);
        }
        #endregion
        #region Private methods
        /// <summary>
        /// Builds an MethodCall using IMethodCallMessage.       
        /// </summary>
        /// <param name="msg">Information, to create a MethodCallRequest</param>
        /// <returns>A new instance of methodCallrequest</returns>
        private MethodCallRequest ToMethodCallRequest(IMethodCallMessage msg)
        {
            Guid id = Guid.NewGuid();
            string methodName = msg.MethodName;
            Dictionary<string, string> metaData = new Dictionary<string, string>();
            //The structure is always domain.DOMAINTYPE.events
            metaData.Add("serviceId", "domain." + domainType + ".events");

            // Arbitrary string, maybe not necessary
            metaData.Add("contextId", "foo");

            List<string> classes = new List<string>();
            //RealClassImplementation is optinal
            List<string> realClassImplementation = new List<string>();
            foreach (object arg in msg.Args)
            {
                String namesp = arg.GetType().Namespace;
                LocalType type = new LocalType(arg.GetType());
                realClassImplementation.Add(type.RemoteTypeFullName);
                classes.Add(type.RemoteTypeFullName);
            }
            RemoteMethodCall call = RemoteMethodCall.CreateInstance(methodName, msg.Args, metaData, classes, realClassImplementation);

            Data data = Data.CreateInstance(username, password);
            Authentification authentification = Authentification.createInstance(AUTHENTIFICATION_CLASS, data, BinaryData.CreateInstance());
            Message message = Message.createInstance(call, id.ToString(), true, "");
            return MethodCallRequest.CreateInstance(authentification, message);
        }
        #endregion
    }
}
