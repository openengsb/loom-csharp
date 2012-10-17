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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote
{
    /// <summary>
    /// This class builds reverse proxies for resources (class instances) on the
    /// client side for the bus.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DomainReverseProxy<T> : DomainReverse<T>
    {
        protected override string CREATION_METHOD_NAME
        {
            get { return "createWithId"; }
        }
        protected override string AUTHENTIFICATION_CLASS
        {
            get { return "org.openengsb.connector.usernamepassword.Password"; }
        }
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localDomainService">LocalDomain</param>
        /// <param name="host">Host</param>
        /// <param name="serviceId">ServiceId</param>
        /// <param name="domainType">name of the remote Domain</param>
        /// <param name="domainEvents">Type of the remoteDomainEvents</param>
        public DomainReverseProxy(T localDomainService, string host, string serviceId, string domainType, Boolean createNewConnector,EExceptionHandling exceptionhandle)
            : base(localDomainService, host, serviceId, domainType, createNewConnector,exceptionhandle)
        {
            logger.Info("Connecting to OpenEngSB version 3.0");
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
        public DomainReverseProxy(T localDomainService, string host, string serviceId, string domainType, String username, String password, Boolean createNewConnector, EExceptionHandling exceptionhandling)
            : base(localDomainService, host, serviceId, domainType, username, password, createNewConnector,exceptionhandling)
        {
            logger.Info("Connecting to OpenEngSB version 3.0");
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Creates an Proxy on the bus.
        /// </summary>
        public override void CreateRemoteProxy()
        {
            logger.Info("Create a new connector");
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);
            registerId = serviceId;

            IList<string> classes = new List<string>();
            LocalType localType = new LocalType(typeof(String));
            classes.Add(localType.RemoteTypeFullName);
            classes.Add("org.openengsb.core.api.model.ConnectorDescription");

            IList<object> args = new List<object>();
            Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObject.ConnectorDescription connectorDescription = new Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObject.ConnectorDescription();
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
            String id = Guid.NewGuid().ToString();
            BeanDescription autinfo = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            autinfo.data.Add("value", password);
            MethodCallMessage secureRequest = MethodCallMessage.createInstance(username, autinfo, creationCall, id, true, "");
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination,exceptionhandling);
            string request = marshaller.MarshallObject(secureRequest);
            portOut.Send(request, id);
            waitAndCheckAnswer(destinationinfo, id);
            registrationprocess = ERegistration.CREATED;
            portOut.Close();
            logger.Info("Create done");
        }

        /// <summary>
        /// Deletes the created remote proxy on the bus.
        /// </summary>
        public override void DeleteRemoteProxy()
        {
            logger.Info("Delete the connector with ID: " + serviceId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);

            IList<object> args = new List<object>();
            args.Add(registerId);

            RemoteMethodCall deletionCall = RemoteMethodCall.CreateInstance(CREATION_DELETE_METHOD_NAME, args, metaData, classes, null);

            String id = Guid.NewGuid().ToString();
            BeanDescription authentification = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            authentification.data.Add("value", password);
            MethodCallMessage callRequest = MethodCallMessage.createInstance(username, authentification, deletionCall, id, true, "");

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, exceptionhandling);
            string request = marshaller.MarshallObject(callRequest);
            portOut.Send(request, id);

            waitAndCheckAnswer(destinationinfo, id);
            registrationprocess = ERegistration.NONE;
            portOut.Close();
            logger.Info("Delete done");
        }

        /// <summary>
        /// Creates an Proxy on the bus.
        /// </summary>
        public override void RegisterConnector(String serviceId)
        {
            this.serviceId = serviceId;
            logger.Info("Register the connector with ID: " + serviceId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            String id= Guid.NewGuid().ToString();
            metaData.Add("serviceId", CREATION_REGISTRATION);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            IList<object> args = new List<object>();
            args.Add(serviceId);
            args.Add(CREATION_PORT);
            args.Add(destination);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(REGISTRATION_METHOD_NAME, args, metaData, classes, null);

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            BeanDescription autinfo = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            autinfo.data.Add("value", password);

            MethodCallMessage secureRequest = MethodCallMessage.createInstance(username, autinfo, creationCall,id, true, "");
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination,exceptionhandling);
            string request = marshaller.MarshallObject(secureRequest);
            portOut.Send(request,id);
            waitAndCheckAnswer(destinationinfo, id);
            registrationprocess = ERegistration.REGISTERED;
            portOut.Close();
            logger.Info("Register done");
        }
        private MethodResultMessage waitAndCheckAnswer(Destination destinationinfo, String id)
        {
            IIncomingPort portIn = new JmsIncomingPort(Destination.CreateDestinationString(destinationinfo.Host, id), exceptionhandling);
            string reply = portIn.Receive();
            MethodResultMessage result = marshaller.UnmarshallObject<MethodResultMessage>(reply);
            portIn.Close();
            if (result.result.type == ReturnType.Exception)
                throw new OpenEngSBException("Remote Exception while Registering service proxy", new Exception(result.result.className));
            return result;
        }
        /// <summary>
        /// Creates an Proxy on the bus.
        /// </summary>
        public override void UnRegisterConnector()
        {
            if (!registrationprocess.Equals(ERegistration.REGISTERED))
            {
                return;
            }
            logger.Info("Unregister the connector with ID: " + serviceId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_REGISTRATION);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);

            IList<object> args = new List<object>();
            args.Add(serviceId);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(UNREGISTRATION_METHOD_NAME, args, metaData, classes, null);

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            BeanDescription autinfo = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            autinfo.data.Add("value", password);
            String id=Guid.NewGuid().ToString();
            MethodCallMessage secureRequest = MethodCallMessage.createInstance(username, autinfo, creationCall, id, true, "");
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, exceptionhandling);
            string request = marshaller.MarshallObject(secureRequest);
            portOut.Send(request,id);
            waitAndCheckAnswer(destinationinfo, id);
            if (registrationprocess.Equals(ERegistration.REGISTERED))
            {
                registrationprocess = ERegistration.CREATED;
            }
            portOut.Close();
            logger.Info("Unregister done");
        }
        /// <summary>
        /// Blocks an waits for messages.
        /// </summary>
        public override void Listen()
        {
            while (isEnabled)
            {
                String textMsg;
                try
                {
                    textMsg = portIn.Receive();
                }
                catch (Exception e)
                {
                    if (this.exceptionhandling == EExceptionHandling.Retry)
                    {
                        this.RegisterConnector(this.serviceId);
                        continue;
                    }
                    else
                    {
                        throw e;
                    }
                }

                if (textMsg == null)
                    continue;
                MethodCallMessage methodCallRequest = marshaller.UnmarshallObject<MethodCallMessage>(textMsg);
                if (methodCallRequest.methodCall.args == null)
                    methodCallRequest.methodCall.args = new List<Object>();
                MethodResultMessage methodReturnMessage = CallMethod(methodCallRequest);

                if (methodCallRequest.answer)
                {
                    string returnMsg = marshaller.MarshallObject(methodReturnMessage);
                    Destination dest = new Destination(destination);
                    IOutgoingPort portOut = new JmsOutgoingPort(Destination.CreateDestinationString(dest.Host, methodCallRequest.callId), exceptionhandling);
                    portOut.Send(returnMsg);
                    if (methodReturnMessage.result.type.Equals(ReturnType.Exception))
                        throw new BridgeException("A exception occurs, while the message has been created", new BridgeException(methodReturnMessage.result.arg.ToString()));
                }
            }
        }
        /// <summary>
        /// Connect a connector to xlink
        /// </summary>
        /// <param name="ServiceId"></param>
        /// <param name="hostId"></param>
        /// <param name="toolName"></param>
        /// <param name="modelsToViews"></param>
        /// <returns></returns>
        public override XLinkTemplate ConnectToXLink(string toolName, ModelToViewsTuple[] modelsToViews)
        {
            logger.Info("Create a Xlink connector");
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);

            IList<string> classes = new List<string>();
            LocalType localType = new LocalType(typeof(String));
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            localType = new LocalType(modelsToViews.GetType());
            classes.Add(localType.RemoteTypeFullName);

            IList<object> args = new List<object>();
            args.Add(registerId);
            args.Add(getHost());
            args.Add(toolName);
            args.Add(modelsToViews);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(XLINK_METHOD_NAME, args, metaData, classes, null);

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;
            String id = Guid.NewGuid().ToString();
            BeanDescription autinfo = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            autinfo.data.Add("value", password);
            MethodCallMessage methodCall = MethodCallMessage.createInstance(username, autinfo, creationCall, id, true, "");
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, exceptionhandling);
            string request = marshaller.MarshallObject(methodCall);
            portOut.Send(request, id);
            MethodResultMessage result = waitAndCheckAnswer(destinationinfo, id);
            registrationprocess = ERegistration.Xlink;
            logger.Info("Create done");
            return marshaller.UnmarshallObject<XLinkTemplate>(result.result.arg.ToString());
        }
        /// <summary>
        /// Disconnect the Connector from XLink
        /// </summary>
        public override void DisconnectFromXLink()
        {
            logger.Info("Disconnect connector from xlink with the serviceId: " + serviceId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);
            LocalType localType = new LocalType(typeof(String));
            IList<String> classes = new List<String>();
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            IList<object> args = new List<object>();
            args.Add(registerId);
            args.Add(getHost());

            RemoteMethodCall deletionCall = RemoteMethodCall.CreateInstance(REMOVE_XLINK_CONNECTOR, args, metaData, classes, null);

            String id = Guid.NewGuid().ToString();
            BeanDescription authentification = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            authentification.data.Add("value", password);
            MethodCallMessage callRequest = MethodCallMessage.createInstance(username, authentification, deletionCall, id, true, "");

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, exceptionhandling);
            string request = marshaller.MarshallObject(callRequest);
            portOut.Send(request, id);

            waitAndCheckAnswer(destinationinfo, id);
            registrationprocess = ERegistration.REGISTERED;
            logger.Info("XLink is disconnected");
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
            catch (BridgeException bridgeEx)
            {
                return CreateMethodReturn(ReturnType.Exception, bridgeEx, request.callId);
            }
            MethodResultMessage returnMsg = null;
            if (returnValue == null)
                returnMsg = CreateMethodReturn(ReturnType.Void, null, request.callId);
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
            {
                if (!type.Equals(ReturnType.Exception))
                    methodResult.className = new LocalType(returnValue.GetType()).RemoteTypeFullName;
                else
                    methodResult.className = returnValue.GetType().ToString();
            }
            methodResult.metaData = new Dictionary<string, string>();
            return MethodResultMessage.CreateInstance(methodResult, correlationId);
        }
        #endregion
    }
}