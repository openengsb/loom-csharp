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
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Protocols;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects;
using Bridge.Implementation.Communication.Jms;
using Bridge.Implementation.Communication;
using Bridge.Implementation.Communication.Json;
using Bridge.Implementation.Common;
using Bridge.Implementation;
namespace Bridge.Implementation.OpenEngSB3_0_0.Remote
{
    /// <summary>
    /// This class generates generic proxies. All method calls will be forwared to the configured server.
    /// </summary>
    /// <typeparam name="T">Type to proxy.</typeparam>
    public class DomainProxy<T> : RealProxy
    {
        #region Variables
        /// <summary>
        /// Username for the authentification
        /// </summary>
        private String username;
        /// <summary>
        /// Password for the authentification
        /// </summary>
        private String password;
        /// <summary>
        /// Name of the queue the server listens to for calls.
        /// </summary>
        private const string HOST_QUEUE = "receive";

        /// <summary>
        /// Id identifying the service instance on the bus.
        /// </summary>
        private string serviceId;
        /// <summary>
        /// Domain type
        /// </summary>
        private string domainType;

        /// <summary>
        /// Host string of the server.
        /// </summary>
        private string host;

        private IMarshaller marshaller;
        #endregion
        #region Constructors
        public DomainProxy(string host, string serviceId, String domainType)
            : base(typeof(T))
        {
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.host = host; ;
            this.marshaller = new JsonMarshaller();
            this.username = "admin";
            this.password = "password";
        }
        public DomainProxy(string host, string serviceId, String domainType,String username,String password)
            : base(typeof(T))
        {
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.host = host; ;
            this.marshaller = new JsonMarshaller();
            this.username = username;
            this.password = password;
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
            SecureMethodCallRequest methodCallRequest = ToMethodCallRequest(callMessage);
            string methodCallMsg = marshaller.MarshallObject(methodCallRequest);
            IOutgoingPort portOut = new JmsOutgoingPort(Destination.CreateDestinationString(host, HOST_QUEUE));            
            portOut.Send(methodCallMsg);
            IIncomingPort portIn = new JmsIncomingPort(Destination.CreateDestinationString(host, methodCallRequest.message.callId));
            string methodReturnMsg = portIn.Receive();
            MethodResultMessage methodReturn = marshaller.UnmarshallObject(methodReturnMsg, typeof(MethodResultMessage)) as MethodResultMessage;
            return ToMessage(methodReturn.message.result, callMessage);
        }
        public new T GetTransparentProxy()
        {
            return (T)base.GetTransparentProxy();
        }
        #endregion
        #region Private methods
        /// <summary>
        /// Builds an IMessage using MethodReturn.
        /// </summary>
        /// <param name="methodReturn">Servers return message</param>
        /// <param name="callMessage">Method an parameters</param>
        /// <returns>The result of the Message</returns>
        private IMessage ToMessage(MethodResult methodReturn, IMethodCallMessage callMessage)
        {
            IMethodReturnMessage returnMessage = null;
            switch (methodReturn.type)
            {
                case MethodResult.ReturnType.Exception:
                    returnMessage = new ReturnMessage((Exception)methodReturn.arg, callMessage);
                    break;
                case MethodResult.ReturnType.Void:
                case MethodResult.ReturnType.Object:
                    returnMessage = new ReturnMessage(methodReturn.arg, null, 0, null, callMessage);
                    break;
                default:
                    return null;
            }
            return returnMessage;
        }

       
        /// <summary>
        /// Builds an MethodCall using IMethodCallMessage.
        /// </summary>
        /// <param name="msg">Information, to create a MethodCallRequest</param>
        /// <returns>A new instance of methodCallrequest</returns>
        private SecureMethodCallRequest ToMethodCallRequest(IMethodCallMessage msg)
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
                classes.Add(HelpMethods.GetPackageName(type.RemoteTypeFullName,typeof(T)) + "." + HelpMethods.FirstLetterToUpper(type.RemoteTypeFullName.Replace(namesp+".","")));
            }
            RemoteMethodCall call = RemoteMethodCall.CreateInstance(methodName, msg.Args, metaData, classes,null);
            String classname = "org.openengsb.connector.usernamepassword.Password";
            Data data = Data.CreateInstance("password");
            AuthenticationInfo authentification = AuthenticationInfo.createInstance(classname,data);
            Message message = Message.createInstance(call, id.ToString(), true, "");
            return SecureMethodCallRequest.createInstance("admin",authentification, message);
        }
        #endregion

    }
}
