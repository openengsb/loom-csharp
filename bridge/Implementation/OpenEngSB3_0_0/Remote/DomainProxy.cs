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
using System.Runtime.Remoting.Messaging;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote
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
            AUTHENTIFICATION_CLASS = "org.openengsb.connector.usernamepassword.Password";
        }
        public DomainProxy(string host, string serviceId, String domainType, String username, String password)
            : base(host, serviceId, domainType, username, password)
        {
            AUTHENTIFICATION_CLASS = "org.openengsb.connector.usernamepassword.Password";
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
            string methodCallMsg = marshaller.MarshallObject(methodCallRequest);
            IOutgoingPort portOut = new JmsOutgoingPort(Destination.CreateDestinationString(host, HOST_QUEUE));
            portOut.Send(methodCallMsg, methodCallRequest.callId);
            IIncomingPort portIn = new JmsIncomingPort(Destination.CreateDestinationString(host, methodCallRequest.callId));
            string methodReturnMsg = portIn.Receive();
            MethodResultMessage methodReturn = marshaller.UnmarshallObject<MethodResultMessage>(methodReturnMsg);
            return ToMessage(methodReturn.result, callMessage);
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
            //The structure is always domain.DOMAINTYPE.events
            metaData.Add("serviceId", "domain." + domainType + ".events");
            // Arbitrary string, maybe not necessary
            metaData.Add("contextId", "foo");
            List<string> classes = new List<string>();
            foreach (object arg in msg.Args)
            {
                String namesp = arg.GetType().Namespace;
                LocalType type = new LocalType(arg.GetType());
                classes.Add(type.RemoteTypeFullName);
            }
            RemoteMethodCall call = RemoteMethodCall.CreateInstance(methodName, msg.Args, metaData, classes, null);
            BeanDescription authentification = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            authentification.data.Add("value", password);
            MethodCallMessage message = MethodCallMessage.createInstance(username, authentification, call, id.ToString(), true, "");
            return message;
        }
        #endregion

    }
}
