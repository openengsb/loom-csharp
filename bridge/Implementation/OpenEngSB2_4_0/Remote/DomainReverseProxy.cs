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
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
 * See the License for the specific language governing permissions and
 * limitations under the License.
 ***/
using System;
using System.Collections.Generic;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB2_4_0.Remote.RemoteObjects;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB2_4_0.Remote
{
    /// <summary>
    /// This class builds reverse proxies for resources (class instances) on the
    /// client side for the bus.
    /// </summary>
    /// <typeparam name="T">Type of the Domain</typeparam>
    public class DomainReverseProxy<T> : DomainReverse<T>
    {
        #region Propreties
        protected override string CREATION_METHOD_NAME
        {
            get { return "create"; }
        }
        protected override string AUTHENTIFICATION_CLASS
        {
            get { return "org.openengsb.core.api.security.model.UsernamePasswordAuthenticationInfo"; }
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
        public DomainReverseProxy(T localDomainService, string host, string connectorId, string domainName, ABridgeExceptionHandling exceptionhandling)
            : base(localDomainService, host, connectorId, domainName, false, exceptionhandling)
        {
            Logger.Info("Connecting to OpenEngSB version 2.4");
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
        public DomainReverseProxy(T localDomainService, string host, string connectorId, string domainName, String username, String password, ABridgeExceptionHandling exceptionhandling)
            : base(localDomainService, host, connectorId, domainName, username, password, false, exceptionhandling)
        {
            Logger.Info("Connecting to OpenEngSB version 2.4");
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
            Guid id = Guid.NewGuid();

            Data data = Data.CreateInstance(Username, Password);
            Authentification authentification = Authentification.createInstance(AUTHENTIFICATION_CLASS, data, BinaryData.CreateInstance());

            IList<string> classes = new List<string>();
            classes.Add("org.openengsb.core.api.model.ConnectorId");
            classes.Add("org.openengsb.core.api.model.ConnectorDescription");

            IList<object> args = new List<object>();
            Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects.ConnectorDescription connectorDescription = new Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects.ConnectorDescription();
            connectorDescription.attributes.Add("serviceId", ConnectorId);
            connectorDescription.attributes.Add("portId", CREATION_PORT);
            connectorDescription.attributes.Add("destination", destination);

            ConnectorId connectorId = new ConnectorId();
            connectorId.connectorType = CREATION_CONNECTOR_TYPE;
            connectorId.instanceId = ConnectorId;
            connectorId.domainType = DomainName;
            RegisterId = connectorId;
            args.Add(connectorId);
            args.Add(connectorDescription);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(CREATION_METHOD_NAME, args, metaData, classes, null);


            Message message = Message.createInstance(creationCall, id.ToString(), true, "");
            MethodCallRequest callRequest = MethodCallRequest.CreateInstance(authentification, message);
            callRequest.message.methodCall = creationCall;

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler);
            string request = Marshaller.MarshallObject(callRequest);
            portOut.Send(request);
            Registrationprocess = ERegistration.REGISTERED;
            portOut.Close();
        }

        /// <summary>
        /// Deletes the created remote proxy on the bus.
        /// </summary>
        public override void DeleteRemoteProxy()
        {
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);

            IList<string> classes = new List<string>();
            classes.Add("org.openengsb.core.api.model.ConnectorId");

            IList<object> args = new List<object>();
            args.Add((ConnectorId)RegisterId);

            RemoteMethodCall deletionCall = RemoteMethodCall.CreateInstance(CREATION_DELETE_METHOD_NAME, args, metaData, classes, null);

            Guid id = Guid.NewGuid();
            String classname = "org.openengsb.core.api.security.model.UsernamePasswordAuthenticationInfo";
            Data data = Data.CreateInstance(Username, Password);
            Authentification authentification = Authentification.createInstance(classname, data, BinaryData.CreateInstance());

            Message message = Message.createInstance(deletionCall, id.ToString(), true, "");
            MethodCallRequest callRequest = MethodCallRequest.CreateInstance(authentification, message);

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = CREATION_QUEUE;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler);
            string request = Marshaller.MarshallObject(callRequest);
            portOut.Send(request);

            IIncomingPort portIn = new JmsIncomingPort(Destination.CreateDestinationString(destinationinfo.Host, callRequest.message.callId), ExceptionHandler);
            string reply = portIn.Receive();

            MethodResultMessage result = Marshaller.UnmarshallObject<MethodResultMessage>(reply);
            Registrationprocess = ERegistration.NONE;
            if (result.message.result.Type == ReturnType.Exception)
                throw new OpenEngSBException("Remote Exception while deleting service proxy", new BridgeException(result.message.result.className));
        }

        /// <summary>
        /// Blocks an waits for messages.
        /// </summary>
        public override void Listen()
        {
            while (IsEnabled)
            {
                String textMsg = PortIn.Receive();

                if (textMsg == null)
                    continue;

                MethodCallRequest methodCallRequest = Marshaller.UnmarshallObject<MethodCallRequest>(textMsg);
                if (methodCallRequest.message.methodCall.args == null)
                    methodCallRequest.message.methodCall.args = new List<Object>();
                MethodResultMessage methodReturnMessage = CallMethod(methodCallRequest);

                if (methodCallRequest.message.answer)
                {
                    string returnMsg = Marshaller.MarshallObject(methodReturnMessage);
                    Destination dest = new Destination(destination);
                    IOutgoingPort portOut = new JmsOutgoingPort(Destination.CreateDestinationString(dest.Host, methodCallRequest.message.callId), ExceptionHandler);
                    portOut.Send(returnMsg);
                    portOut.Close();
                    if (methodReturnMessage.message.result.Type.Equals(ReturnType.Exception))
                        throw new OpenEngSBException("A problem with the invokation of a method happened", new BridgeException(methodReturnMessage.message.result.Arg.ToString()));
                }
            }
        }
        public override XLinkUrlBlueprint ConnectToXLink(string toolName, string hostId, ModelToViewsTuple[] modelsToViews)
        {
            throw new NotImplementedException();
        }
        public override void DisconnectFromXLink(String hostId)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Calls a method according to MethodCall.
        /// </summary>
        /// <param name="methodCall">Description of the call.</param>
        /// <returns></returns>
        private MethodResultMessage CallMethod(MethodCallRequest request)
        {
            object returnValue = null;
            try
            {
                returnValue = InvokeMethod(request.message.methodCall);
            }
            catch (BridgeException ex)
            {
                return CreateMethodReturn(ReturnType.Exception, ex, request.message.callId);
            }
            MethodResultMessage returnMsg = null;
            if (returnValue == null)
                returnMsg = CreateMethodReturn(ReturnType.Void, "null", request.message.callId);
            else
                returnMsg = CreateMethodReturn(ReturnType.Object, returnValue, request.message.callId);
            return returnMsg;
        }

        /// <summary>
        /// Builds an return message.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="returnValue">Return Value</param>
        /// <param name="correlationId">Correlation Id</param>
        /// <returns></returns>
        private MethodResultMessage CreateMethodReturn(ReturnType type, object returnValue, string correlationId)
        {
            MethodResult methodResult = new MethodResult();
            methodResult.Type = type;
            methodResult.Arg = returnValue;
            MethodResultMessage methodResultMessage = new MethodResultMessage();
            methodResultMessage.message = new MessageResult();
            methodResultMessage.message.callId = correlationId;

            if (returnValue == null)
                methodResult.className = "null";
            else
                methodResult.className = new LocalType(returnValue.GetType()).RemoteTypeFullName;

            methodResult.metaData = new Dictionary<string, string>();

            methodResultMessage.message.result = methodResult;
            return methodResultMessage;
        }
        public override void RegisterConnector(String registerId)
        {
            throw new MissingMethodException("Not implemented for this version of openEngSB");
        }
        public override void UnRegisterConnector()
        {
            throw new MissingMethodException("Not implemented for this version of openEngSB");
        }
        #endregion
    }
}