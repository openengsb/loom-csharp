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
using System.Threading;
using System.Reflection;
using System.IO;
using Implementation.OpenEngSB3_0_0.Remote.RemoteObjects;
using Implementation.Communication.Json;
using Implementation.Communication.Jms;
using Implementation.Communication;
using Implementation.Common;
using Implementation.OpenEngSB3_0_0.Remote.RemoteObject;
using Implementation.Common.Enumeration;
using Implementation.Common.RemoteObjects;

namespace Implementation.OpenEngSB3_0_0.Remote
{
    /// <summary>
    /// This class builds reverse proxies for resources (class instances) on the
    /// client side for the bus.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DomainReverseProxy<T> : DomainReverse<T>, IStoppable
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localDomainService">LocalDomain</param>
        /// <param name="host">Host</param>
        /// <param name="serviceId">ServiceId</param>
        /// <param name="domainType">name of the remote Domain</param>
        /// <param name="domainEvents">Type of the remoteDomainEvents</param>
        public DomainReverseProxy(T localDomainService, string host, string serviceId, string domainType)
            : base(localDomainService, host, serviceId, domainType)
        {
            logger.Info("Connecting to OpenEngSB version 3.0");
            CREATION_METHOD_NAME = "createWithId";
            AUTHENTIFICATION_CLASS = "org.openengsb.connector.usernamepassword.Password";
        }
        /// <summary>
        /// Constructor with Autehntification
        /// </summary>
        /// <param name="localDomainService">LocalDomain</param>
        /// <param name="host">Host</param>
        /// <param name="serviceId">ServiceId</param>
        /// <param name="domainType">name of the remote Domain</param>
        /// <param name="username">Username for the authentification</param>
        /// <param name="password">Password for the authentification</param>
        public DomainReverseProxy(T localDomainService, string host, string serviceId, string domainType, String username, String password)
            : base(localDomainService, host, serviceId, domainType, username, password)
        {
            logger.Info("Connecting to OpenEngSB version 3.0");
            CREATION_METHOD_NAME = "createWithId";
            AUTHENTIFICATION_CLASS = "org.openengsb.connector.usernamepassword.Password";
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Creates an Proxy on the bus.
        /// </summary>
        public override void CreateRemoteProxy()
        {
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);
            registerId = Guid.NewGuid().ToString();


            IList<string> classes = new List<string>();
            LocalType localType = new LocalType(typeof(String));
            classes.Add(localType.RemoteTypeFullName);
            classes.Add("org.openengsb.core.api.model.ConnectorDescription");

            IList<object> args = new List<object>();
            ConnectorDescription connectorDescription = new ConnectorDescription();
            connectorDescription.attributes.Add("serviceId", serviceId);
            connectorDescription.attributes.Add("portId", CREATION_PORT);
            connectorDescription.attributes.Add("destination", destination);
            connectorDescription.connectorType = CREATION_CONNECTOR_TYPE;
            connectorDescription.domainType = domainType;

            args.Add(registerId);
            args.Add(connectorDescription);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(CREATION_METHOD_NAME, args, metaData, classes, null);
            
            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            BeanDescription autinfo = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            autinfo.data.Add("value", password);
            MethodCallMessage secureRequest = MethodCallMessage.createInstance(username, autinfo, creationCall, Guid.NewGuid().ToString(), true, "");
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination);
            string request = marshaller.MarshallObject(secureRequest);
            portOut.Send(request);
        }

        /// <summary>
        /// Deletes the created remote proxy on the bus.
        /// </summary>
        public override void DeleteRemoteProxy()
        {
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);

            IList<object> args = new List<object>();
            args.Add(registerId);

            RemoteMethodCall deletionCall = RemoteMethodCall.CreateInstance(CREATION_DELETE_METHOD_NAME, args, metaData, classes, null);

            Guid id = Guid.NewGuid();
            BeanDescription authentification = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            authentification.data.Add("value", password);
            MethodCallMessage callRequest = MethodCallMessage.createInstance(username, authentification, deletionCall, id.ToString(), true, "");

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination);
            string request = marshaller.MarshallObject(callRequest);
            portOut.Send(request, id.ToString());

            IIncomingPort portIn = new JmsIncomingPort(Destination.CreateDestinationString(destinationinfo.Host, id.ToString()));
            string reply = portIn.Receive();

            MethodResultMessage result = marshaller.UnmarshallObject(reply, typeof(MethodResultMessage)) as MethodResultMessage;
            if (result.result.type == ReturnType.Exception)
                throw new ApplicationException("Remote Exception while deleting service proxy");
        }

        /// <summary>
        /// Blocks an waits for messages.
        /// </summary>
        public override void Listen()
        {
            while (isEnabled)
            {
                String textMsg = portIn.Receive();

                if (textMsg == null)
                    continue;
                MethodCallMessage methodCallRequest = marshaller.UnmarshallObject(textMsg, typeof(MethodCallMessage)) as MethodCallMessage;
                if (methodCallRequest.methodCall.args == null) methodCallRequest.methodCall.args = new List<Object>();

                MethodResultMessage methodReturnMessage = CallMethod(methodCallRequest);

                if (methodCallRequest.answer)
                {
                    string returnMsg = marshaller.MarshallObject(methodReturnMessage);
                    Destination dest = new Destination(destination);
                    IOutgoingPort portOut = new JmsOutgoingPort(Destination.CreateDestinationString(dest.Host, methodCallRequest.callId));
                    portOut.Send(returnMsg);
                    if (methodReturnMessage.result.type.Equals(ReturnType.Exception)) 
                        throw new Exception(methodReturnMessage.result.arg.ToString());
                }
            }
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Calls a method according to MethodCall.
        /// </summary>
        /// <param name="methodCall">Description of the call.</param>
        /// <returns></returns>        
        private MethodResultMessage CallMethod(MethodCallMessage request)
        {                   
            object returnValue = null;
            try
            {
                returnValue = invokeMethod(request.methodCall);
            }
            catch (Exception ex)
            {
                return CreateMethodReturn(ReturnType.Exception, ex, request.callId);
            }

            MethodResultMessage returnMsg = null;

            if (returnValue == null)
                returnMsg = CreateMethodReturn(ReturnType.Void, "null", request.callId);
            else
                returnMsg = CreateMethodReturn(ReturnType.Object, returnValue, request.callId);

            return returnMsg;
        }

        /// <summary>
        /// Builds an return message.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="returnValue"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        private MethodResultMessage CreateMethodReturn(ReturnType type, object returnValue, string correlationId)
        {
            MethodResult methodResult = new MethodResult();
            methodResult.type = type;
            methodResult.arg = returnValue;            
                                  

            if (returnValue == null)
                methodResult.className = "null";
            else
                methodResult.className = new LocalType(returnValue.GetType()).RemoteTypeFullName;

            methodResult.metaData = new Dictionary<string, string>();

            return MethodResultMessage.CreateInstance(methodResult, correlationId);  
        }
        #endregion
    }
}