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
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote
{
    /// <summary>
    /// This class builds reverse proxies for resources (class instances) on the
    /// client side for the bus.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DomainReverseProxy<T> : DomainReverse<T>
    {
        #region Propreties
        protected override string CREATION_METHOD_NAME
        {
            get { return "createWithId"; }
        }
        protected override string AUTHENTIFICATION_CLASS
        {
            get { return "org.openengsb.connector.usernamepassword.Password"; }
        }
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localDomainService">LocalDomain</param>
        /// <param name="host">Host</param>
        /// <param name="connectorId">ServiceId</param>
        /// <param name="domainName">name of the remote Domain</param>
        /// <param name="domainEvents">Type of the remoteDomainEvents</param>
        public DomainReverseProxy(T localDomainService, string host, string connectorId, string domainName, Boolean createNewConnector, ABridgeExceptionHandling exceptionhandler)
            : base(localDomainService, host, connectorId, domainName, createNewConnector, exceptionhandler)
        {
            Logger.Info("Connecting to OpenEngSB version 3.0");
        }
        /// <summary>
        /// Constructor with Autehntification
        /// </summary>
        /// <param name="localDomainService">LocalDomain</param>
        /// <param name="host">Host</param>
        /// <param name="connectorId">ServiceId</param>
        /// <param name="domainName">name of the remote Domain</param>
        /// <param name="username">Username for the authentification</param>
        /// <param name="password">Password for the authentification</param>
        public DomainReverseProxy(T localDomainService, string host, string connectorId, string domainName, String username, String password, Boolean createNewConnector, ABridgeExceptionHandling exceptionhandler)
            : base(localDomainService, host, connectorId, domainName, username, password, createNewConnector, exceptionhandler)
        {
            Logger.Info("Connecting to OpenEngSB version 3.0");
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Creates an Proxy on the bus.
        /// </summary>
        public override void CreateRemoteProxy()
        {
            Logger.Info("Create a new connector");
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);
            RegisterId = ConnectorId;

            IList<string> classes = new List<string>();
            LocalType localType = new LocalType(typeof(String));
            classes.Add(localType.RemoteTypeFullName);
            classes.Add("org.openengsb.core.api.model.ConnectorDescription");

            IList<object> args = new List<object>();
            Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects.ConnectorDescription connectorDescription = new Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects.ConnectorDescription();
            connectorDescription.attributes.Add("serviceId", ConnectorId);
            connectorDescription.attributes.Add("portId", CREATION_PORT);
            connectorDescription.attributes.Add("destination", destination);
            connectorDescription.connectorType = CREATION_CONNECTOR_TYPE;
            connectorDescription.domainType = DomainName;

            args.Add(RegisterId);
            args.Add(connectorDescription);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(CREATION_METHOD_NAME, args, metaData, classes, null);

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;
            String id = Guid.NewGuid().ToString();
            BeanDescription autinfo = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            autinfo.data.Add("value", Password);
            MethodCallMessage secureRequest = MethodCallMessage.createInstance(Username, autinfo, creationCall, id, true, "");
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(secureRequest);
            portOut.Send(request, id);
            waitAndCheckAnswer(destinationinfo, id);
            Registrationprocess = ERegistration.CREATED;
            portOut.Close();
            Logger.Info("Create done");
        }

        /// <summary>
        /// Deletes the created remote proxy on the bus.
        /// </summary>
        public override void DeleteRemoteProxy()
        {
            Logger.Info("Delete the connector with ID: " + ConnectorId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);

            IList<object> args = new List<object>();
            args.Add(RegisterId);

            RemoteMethodCall deletionCall = RemoteMethodCall.CreateInstance(CREATION_DELETE_METHOD_NAME, args, metaData, classes, null);

            String id = Guid.NewGuid().ToString();
            BeanDescription authentification = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            authentification.data.Add("value", Password);
            MethodCallMessage callRequest = MethodCallMessage.createInstance(Username, authentification, deletionCall, id, true, "");

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(callRequest);
            portOut.Send(request, id);

            waitAndCheckAnswer(destinationinfo, id);
            Registrationprocess = ERegistration.NONE;
            portOut.Close();
            Logger.Info("Delete done");
        }

        /// <summary>
        /// Register a connector on the OpenEngSB
        /// </summary>
        /// <param name="connectorId">ConnectorId</param>
        public override void RegisterConnector(String connectorId)
        {
            this.ConnectorId = connectorId;
            Logger.Info("Register the connector with ID: " + connectorId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            String id = Guid.NewGuid().ToString();
            metaData.Add("serviceId", CREATION_REGISTRATION);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            IList<object> args = new List<object>();
            args.Add(connectorId);
            args.Add(CREATION_PORT);
            args.Add(destination);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(REGISTRATION_METHOD_NAME, args, metaData, classes, null);

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            BeanDescription autinfo = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            autinfo.data.Add("value", Password);

            MethodCallMessage secureRequest = MethodCallMessage.createInstance(Username, autinfo, creationCall, id, true, "");
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(secureRequest);
            portOut.Send(request, id);
            waitAndCheckAnswer(destinationinfo, id);
            Registrationprocess = ERegistration.REGISTERED;
            portOut.Close();
            Logger.Info("Register done");
        }
        private MethodResultMessage waitAndCheckAnswer(Destination destinationinfo, String id)
        {
            IIncomingPort portIn = new JmsIncomingPort(Destination.CreateDestinationString(destinationinfo.Host, id), ExceptionHandler, ConnectorId);
            string reply = portIn.Receive();
            MethodResultMessage result = Marshaller.UnmarshallObject<MethodResultMessage>(reply);
            portIn.Close();
            if (result.result.type == ReturnType.Exception)
            {
                throw new OpenEngSBException("Remote Exception while Registering service proxy", new Exception(result.result.className));
            }
            return result;
        }
        /// <summary>
        /// Creates an Proxy on the bus.
        /// </summary>
        public override void UnRegisterConnector()
        {
            if (!Registrationprocess.Equals(ERegistration.REGISTERED))
            {
                return;
            }
            Logger.Info("Unregister the connector with ID: " + ConnectorId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_REGISTRATION);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);

            IList<object> args = new List<object>();
            args.Add(ConnectorId);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(UNREGISTRATION_METHOD_NAME, args, metaData, classes, null);

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            BeanDescription autinfo = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            autinfo.data.Add("value", Password);
            String id = Guid.NewGuid().ToString();
            MethodCallMessage secureRequest = MethodCallMessage.createInstance(Username, autinfo, creationCall, id, true, "");
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(secureRequest);
            portOut.Send(request, id);
            waitAndCheckAnswer(destinationinfo, id);
            if (Registrationprocess.Equals(ERegistration.REGISTERED))
            {
                Registrationprocess = ERegistration.CREATED;
            }
            portOut.Close();
            Logger.Info("Unregister done");
        }
        /// <summary>
        /// Blocks an waits for messages.
        /// </summary>
        public override void Listen()
        {
            try
            {
                while (IsEnabled)
                {
                    String textMsg;
                    textMsg = PortIn.Receive();

                    if (textMsg == null)
                    {
                        continue;
                    }
                    MethodCallMessage methodCallRequest = Marshaller.UnmarshallObject<MethodCallMessage>(textMsg);
                    if (methodCallRequest.methodCall.args == null)
                    {
                        methodCallRequest.methodCall.args = new List<Object>();
                    }
                    MethodResultMessage methodReturnMessage = CallMethod(methodCallRequest);

                    if (methodCallRequest.answer)
                    {
                        string returnMsg = Marshaller.MarshallObject(methodReturnMessage);
                        Destination dest = new Destination(destination);
                        IOutgoingPort portOut = new JmsOutgoingPort(Destination.CreateDestinationString(dest.Host, methodCallRequest.callId), ExceptionHandler, ConnectorId);
                        portOut.Send(returnMsg);
                        portOut.Close();
                        if (methodReturnMessage.result.type.Equals(ReturnType.Exception))
                        {
                            throw new BridgeException("A exception occurs, while the message has been created", new BridgeException(methodReturnMessage.result.arg.ToString()));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (IsEnabled)
                {
                    ExceptionHandler.Changed += delegate(object[] obj)
                    {
                        Listen();
                        return null;
                    };
                    ExceptionHandler.HandleException(e);
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
        public override XLinkUrlBlueprint ConnectToXLink(string toolName, String hostId, ModelToViewsTuple[] modelsToViews)
        {
            Logger.Info("Create a Xlink connector");
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
            args.Add(RegisterId);
            args.Add(hostId);
            args.Add(toolName);
            args.Add(modelsToViews);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(XLINK_METHOD_NAME, args, metaData, classes, null);

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;
            String id = Guid.NewGuid().ToString();
            BeanDescription autinfo = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            autinfo.data.Add("value", Password);
            MethodCallMessage methodCall = MethodCallMessage.createInstance(Username, autinfo, creationCall, id, true, "");
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(methodCall);
            portOut.Send(request, id);
            portOut.Close();
            MethodResultMessage result = waitAndCheckAnswer(destinationinfo, id);
            Registrationprocess = ERegistration.XLINK;
            Logger.Info("Create done");
            return Marshaller.UnmarshallObject<XLinkUrlBlueprint>(result.result.arg.ToString());
        }
        /// <summary>
        /// Disconnect the Connector from XLink
        /// </summary>
        public override void DisconnectFromXLink(String hostId)
        {
            Logger.Info("Disconnect connector from xlink with the serviceId: " + ConnectorId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);
            LocalType localType = new LocalType(typeof(String));
            IList<String> classes = new List<String>();
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            IList<object> args = new List<object>();
            args.Add(RegisterId);
            args.Add(hostId);

            RemoteMethodCall deletionCall = RemoteMethodCall.CreateInstance(REMOVE_XLINK_CONNECTOR, args, metaData, classes, null);

            String id = Guid.NewGuid().ToString();
            BeanDescription authentification = BeanDescription.createInstance(AUTHENTIFICATION_CLASS);
            authentification.data.Add("value", Password);
            MethodCallMessage callRequest = MethodCallMessage.createInstance(Username, authentification, deletionCall, id, true, "");

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(callRequest);
            portOut.Send(request, id);
            waitAndCheckAnswer(destinationinfo, id);
            portOut.Close();
            Registrationprocess = ERegistration.REGISTERED;
            Logger.Info("XLink is disconnected");
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
                returnValue = InvokeMethod(request.methodCall);
            }
            catch (BridgeException bridgeEx)
            {
                return CreateMethodReturn(ReturnType.Exception, bridgeEx, request.callId);
            }
            MethodResultMessage returnMsg = null;
            if (returnValue == null)
            {
                returnMsg = CreateMethodReturn(ReturnType.Void, null, request.callId);
            }
            else
            {
                returnMsg = CreateMethodReturn(ReturnType.Object, returnValue, request.callId);
            }
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
            {
                methodResult.className = "null";
            }
            else
            {
                if (!type.Equals(ReturnType.Exception))
                {
                    methodResult.className = new LocalType(returnValue.GetType()).RemoteTypeFullName;
                }
                else
                {
                    methodResult.className = returnValue.GetType().ToString();
                }
            }
            methodResult.metaData = new Dictionary<string, string>();
            return MethodResultMessage.CreateInstance(methodResult, correlationId);
        }
        #endregion
    }
}