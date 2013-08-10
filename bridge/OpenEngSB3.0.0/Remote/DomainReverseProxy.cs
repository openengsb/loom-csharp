#region Copyright
// <copyright file="DomainReverseProxy.cs" company="OpenEngSB">
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
using System.Collections.Generic;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;
using Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects;

namespace Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote
{
    /// <summary>
    /// This class builds reverse proxies for resources (class instances) on the
    /// client side for the bus.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DomainReverseProxy<T> : DomainReverse<T>
    {
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

        protected override string AuthentificationClass
        {
            get
            {
                return "org.openengsb.connector.usernamepassword.Password";
            }
        }

        protected override string CreationMethodName
        {
            get
            {
                return "createWithId";
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
            metaData.Add("serviceId", CreationServiceId);

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

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(XlinkMethodName, args, metaData, classes, null);

            JmsDestination destinationinfo = new JmsDestination(Destination);
            destinationinfo.Queue = CreationQueue;
            String id = Guid.NewGuid().ToString();
            BeanDescription autinfo = BeanDescription.CreateInstance(AuthentificationClass);
            autinfo.Data.Add("value", Password);
            MethodCallMessage methodCall = MethodCallMessage.CreateInstance(Username, autinfo, creationCall, id, true, String.Empty);
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(methodCall);
            portOut.Send(request, id);
            portOut.Close();
            MethodResultMessage result = WaitAndCheckAnswer(destinationinfo, id);
            RegistrationProcess = ERegistration.XLINK;
            Logger.Info("Create done");
            return Marshaller.UnmarshallObject<XLinkUrlBlueprint>(result.Result.Arg.ToString());
        }

        /// <summary>
        /// Creates an Proxy on the bus.
        /// </summary>
        public override void CreateRemoteProxy()
        {
            Logger.Info("Create a new connector");
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CreationServiceId);
            RegisterId = ConnectorId;

            IList<string> classes = new List<string>();
            LocalType localType = new LocalType(typeof(String));
            classes.Add(localType.RemoteTypeFullName);
            classes.Add("org.openengsb.core.api.model.ConnectorDescription");

            IList<object> args = new List<object>();
            Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects.ConnectorDescription connectorDescription = new Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects.ConnectorDescription();
            connectorDescription.Attributes.Add("serviceId", ConnectorId);
            connectorDescription.Attributes.Add("portId", CreationPort);
            connectorDescription.Attributes.Add("destination", Destination);
            connectorDescription.ConnectorType = CreationConnectorType;
            connectorDescription.DomainType = DomainName;

            args.Add(RegisterId);
            args.Add(connectorDescription);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(CreationMethodName, args, metaData, classes, null);

            JmsDestination destinationinfo = new JmsDestination(Destination);
            destinationinfo.Queue = CreationQueue;
            String id = Guid.NewGuid().ToString();
            BeanDescription autinfo = BeanDescription.CreateInstance(AuthentificationClass);
            autinfo.Data.Add("value", Password);
            MethodCallMessage secureRequest = MethodCallMessage.CreateInstance(Username, autinfo, creationCall, id, true, String.Empty);
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(secureRequest);
            portOut.Send(request, id);
            WaitAndCheckAnswer(destinationinfo, id);
            RegistrationProcess = ERegistration.CREATED;
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
            metaData.Add("serviceId", CreationServiceId);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);

            IList<object> args = new List<object>();
            args.Add(RegisterId);

            RemoteMethodCall deletionCall = RemoteMethodCall.CreateInstance(CreationDeleteMethodName, args, metaData, classes, null);

            String id = Guid.NewGuid().ToString();
            BeanDescription authentification = BeanDescription.CreateInstance(AuthentificationClass);
            authentification.Data.Add("value", Password);
            MethodCallMessage callRequest = MethodCallMessage.CreateInstance(Username, authentification, deletionCall, id, true, String.Empty);

            JmsDestination destinationinfo = new JmsDestination(Destination);
            destinationinfo.Queue = CreationQueue;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(callRequest);
            portOut.Send(request, id);

            WaitAndCheckAnswer(destinationinfo, id);
            RegistrationProcess = ERegistration.NONE;
            portOut.Close();
            Logger.Info("Delete done");
        }

        /// <summary>
        /// Disconnect the Connector from XLink
        /// </summary>
        public override void DisconnectFromXLink(String hostId)
        {
            Logger.Info("Disconnect connector from xlink with the serviceId: " + ConnectorId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CreationServiceId);
            LocalType localType = new LocalType(typeof(String));
            IList<String> classes = new List<String>();
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            IList<object> args = new List<object>();
            args.Add(RegisterId);
            args.Add(hostId);

            RemoteMethodCall deletionCall = RemoteMethodCall.CreateInstance(RemoveXlinkConnectorMethodName, args, metaData, classes, null);

            String id = Guid.NewGuid().ToString();
            BeanDescription authentification = BeanDescription.CreateInstance(AuthentificationClass);
            authentification.Data.Add("value", Password);
            MethodCallMessage callRequest = MethodCallMessage.CreateInstance(Username, authentification, deletionCall, id, true, String.Empty);

            JmsDestination destinationinfo = new JmsDestination(Destination);
            destinationinfo.Queue = CreationQueue;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(callRequest);
            portOut.Send(request, id);
            WaitAndCheckAnswer(destinationinfo, id);
            portOut.Close();
            RegistrationProcess = ERegistration.REGISTERED;
            Logger.Info("XLink is disconnected");
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
                    if (methodCallRequest.MethodCall.Args == null)
                    {
                        methodCallRequest.MethodCall.Args = new List<Object>();
                    }
                    
                    MethodResultMessage methodReturnMessage = CallMethod(methodCallRequest);
                    if (methodCallRequest.Answer)
                    {
                        string returnMsg = Marshaller.MarshallObject(methodReturnMessage);
                        JmsDestination dest = new JmsDestination(Destination);
                        IOutgoingPort portOut = new JmsOutgoingPort(JmsDestination.CreateDestinationString(dest.Host, methodCallRequest.CallId), ExceptionHandler, ConnectorId);
                        portOut.Send(returnMsg);
                        portOut.Close();
                        if (methodReturnMessage.Result.Type.Equals(ReturnType.Exception))
                        {
                            throw new BridgeException("A exception occurs, while the message has been created", new BridgeException(methodReturnMessage.Result.Arg.ToString()));
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
        /// Register a connector on the OpenEngSB
        /// </summary>
        /// <param name="connectorId">ConnectorId</param>
        public override void RegisterConnector(String connectorId)
        {
            this.ConnectorId = connectorId;
            Logger.Info("Register the connector with ID: " + connectorId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            String id = Guid.NewGuid().ToString();
            metaData.Add("serviceId", CreationRegistration);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            classes.Add(localType.RemoteTypeFullName);
            IList<object> args = new List<object>();
            args.Add(connectorId);
            args.Add(CreationPort);
            args.Add(Destination);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(RegistrationMethodName, args, metaData, classes, null);

            JmsDestination destinationinfo = new JmsDestination(Destination);
            destinationinfo.Queue = CreationQueue;

            BeanDescription autinfo = BeanDescription.CreateInstance(AuthentificationClass);
            autinfo.Data.Add("value", Password);

            MethodCallMessage secureRequest = MethodCallMessage.CreateInstance(Username, autinfo, creationCall, id, true, String.Empty);
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(secureRequest);
            portOut.Send(request, id);
            WaitAndCheckAnswer(destinationinfo, id);
            RegistrationProcess = ERegistration.REGISTERED;
            portOut.Close();
            Logger.Info("Register done");
        }

        /// <summary>
        /// Creates an Proxy on the bus.
        /// </summary>
        public override void UnRegisterConnector()
        {
            if (!RegistrationProcess.Equals(ERegistration.REGISTERED))
            {
                return;
            }

            Logger.Info("Unregister the connector with ID: " + ConnectorId);
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CreationRegistration);
            LocalType localType = new LocalType(typeof(String));
            IList<string> classes = new List<string>();
            classes.Add(localType.RemoteTypeFullName);

            IList<object> args = new List<object>();
            args.Add(ConnectorId);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(UnregistrationMethodName, args, metaData, classes, null);

            JmsDestination destinationinfo = new JmsDestination(Destination);
            destinationinfo.Queue = CreationQueue;

            BeanDescription autinfo = BeanDescription.CreateInstance(AuthentificationClass);
            autinfo.Data.Add("value", Password);
            String id = Guid.NewGuid().ToString();
            MethodCallMessage secureRequest = MethodCallMessage.CreateInstance(Username, autinfo, creationCall, id, true, String.Empty);
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination, ExceptionHandler, ConnectorId);
            string request = Marshaller.MarshallObject(secureRequest);
            portOut.Send(request, id);
            WaitAndCheckAnswer(destinationinfo, id);
            if (RegistrationProcess.Equals(ERegistration.REGISTERED))
            {
                RegistrationProcess = ERegistration.CREATED;
            }

            portOut.Close();
            Logger.Info("Unregister done");
        }

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
                returnValue = InvokeMethod(request.MethodCall);
            }
            catch (BridgeException bridgeEx)
            {
                return CreateMethodReturn(ReturnType.Exception, bridgeEx, request.CallId);
            }

            MethodResultMessage returnMsg = null;
            if (returnValue == null)
            {
                returnMsg = CreateMethodReturn(ReturnType.Void, null, request.CallId);
            }
            else
            {
                returnMsg = CreateMethodReturn(ReturnType.Object, returnValue, request.CallId);
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
            methodResult.Type = type;
            methodResult.Arg = returnValue;

            if (returnValue == null)
            {
                methodResult.ClassName = "null";
            }
            else
            {
                if (!type.Equals(ReturnType.Exception))
                {
                    methodResult.ClassName = new LocalType(returnValue.GetType()).RemoteTypeFullName;
                }
                else
                {
                    methodResult.ClassName = returnValue.GetType().ToString();
                }
            }

            methodResult.MetaData = new Dictionary<string, string>();
            return MethodResultMessage.CreateInstance(methodResult, correlationId);
        }

        private MethodResultMessage WaitAndCheckAnswer(JmsDestination destinationinfo, String id)
        {
            IIncomingPort portIn = new JmsIncomingPort(JmsDestination.CreateDestinationString(destinationinfo.Host, id), ExceptionHandler, ConnectorId);
            string reply = portIn.Receive();
            MethodResultMessage result = Marshaller.UnmarshallObject<MethodResultMessage>(reply);
            portIn.Close();
            if (result.Result.Type == ReturnType.Exception)
            {
                throw new OpenEngSBException("Remote Exception while Registering service proxy", new Exception(result.Result.ClassName));
            }

            return result;
        }
    }
}