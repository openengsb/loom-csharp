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
using System.Linq;
using System.Reflection;
using System.Threading;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.RemoteObjects;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using log4net;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using System.Collections;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    public abstract class DomainReverse<T> : IRegistration
    {
        #region Const.
        protected const string CREATION_QUEUE = "receive";
        protected const string CREATION_SERVICE_ID = "connectorManager";
        protected const string CREATION_REGISTRATION = "proxyConnectorRegistry";
        protected const string CREATION_DELETE_METHOD_NAME = "delete";
        protected const string REMOVE_XLINK_CONNECTOR = "disconnectFromXLink";
        protected const string CREATION_PORT = "jms-json";
        protected const string CREATION_CONNECTOR_TYPE = "external-connector-proxy";
        protected const string REGISTRATION_METHOD_NAME = "registerConnector";
        protected const string UNREGISTRATION_METHOD_NAME = "unregisterConnector";
        protected const string XLINK_METHOD_NAME = "connectToXLink";
        #endregion
        #region Propreties
        protected abstract string CREATION_METHOD_NAME { get; }
        protected abstract string AUTHENTIFICATION_CLASS { get; }
        public string ConnectorId { get { return connectorId; }
            set { connectorId = value; }
        }
        public String DomainName { get { return domainName; } }
        #endregion
        #region Variables

        public ABridgeExceptionHandling ExceptionHandler;
        /// <summary>
        /// indicates in witch state the registration is
        /// </summary>
        protected ERegistration Registrationprocess = ERegistration.NONE;
        /// <summary>
        ///Descrips if the Registration has been done.
        /// </summary>
        public Boolean Registered
        {
            get { return Registrationprocess.Equals(ERegistration.REGISTERED); }
        }
        /// <summary>
        /// Defines if the connector should create a new connector or if it should register a existing one
        /// </summary>
        protected Boolean CreateService = true;

        /// <summary>
        /// domain-instance to act as reverse-proxy for
        /// </summary>
        private T domainService;
        /// <summary>
        /// Logger
        /// </summary>
        protected static ILog Logger = LogManager.GetLogger(typeof(T));
        #endregion
        #region Propreties
        public T DomainService
        {
            get { return domainService; }
        }
        #endregion
        #region Variabels
        /// <summary>
        /// Username for the authentification
        /// </summary>
        protected String Username;
        /// <summary>
        /// Username for the password
        /// </summary>
        protected String Password;
        // Thread listening for messages
        protected Thread QueueThread;

        // IO port
        protected IIncomingPort PortIn;

        protected String destination;

        /// <summary>
        /// ServiceId of the proxy on the bus
        /// </summary>
        private String connectorId;

        /// <summary>
        ///  DomainType string required for OpenengSb
        /// </summary>
        private String domainName;

        /// <summary>
        /// flag indicating if the listening thread should run
        /// </summary>
        protected Boolean IsEnabled;
        /// <summary>
        /// The used marshaller
        /// </summary>
        protected IMarshaller Marshaller;
        /// <summary>
        /// The id, which has been used to register the connector
        /// </summary>
        protected Object RegisterId;
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localDomainService">LocalDomain</param>
        /// <param name="host">Host</param>
        /// <param name="serviceId">ServiceId</param>
        /// <param name="domainName">name of the remote Domain</param>
        /// <param name="domainEvents">Type of the remoteDomainEvents</param>
        public DomainReverse(T localDomainService, string host, string serviceId, string domainName, Boolean createNewConnector, ABridgeExceptionHandling exceptionhandler)
        {
            this.ExceptionHandler = exceptionhandler;
            this.Marshaller = new JsonMarshaller();
            this.IsEnabled = true;
            this.destination = Destination.CreateDestinationString(host, serviceId);
            this.QueueThread = null;
            this.connectorId = serviceId;
            this.domainName = domainName;
            this.domainService = localDomainService;
            this.PortIn = new JmsIncomingPort(destination, exceptionhandler);
            this.Username = "admin";
            this.Password = "password";
            this.CreateService = createNewConnector;
        }
        /// <summary>
        /// Constructor with Autehntification
        /// </summary>
        /// <param name="localDomainService">LocalDomain</param>
        /// <param name="host">Host</param>
        /// <param name="serviceId">ServiceId</param>
        /// <param name="domainName">name of the remote Domain</param>
        /// <param name="username">Username for the authentification</param>
        /// <param name="password">Password for the authentification</param>
        public DomainReverse(T localDomainService, string host, string serviceId, string domainName, String username, String password, Boolean createNewConnector, ABridgeExceptionHandling exceptionhandler)
            : this(localDomainService, host, serviceId, domainName, createNewConnector, exceptionhandler)
        {
            this.Username = username;
            this.Password = password;
        }
        #endregion
        #region Protected Method
        /// <summary>
        /// Unmarshalls the arguments of a MethodCall.
        /// </summary>
        /// <param name="methodCall">MethodCall</param>
        /// <returns>Arguments</returns>
        protected object[] CreateMethodArguments(IMethodCall methodCall, MethodInfo methodInfo)
        {
            IList<object> args = new List<object>();
            Assembly asm = typeof(T).GetType().Assembly;
            for (int i = 0; i < methodCall.args.Count; ++i)
            {
                Object arg = methodCall.args[i];
                String methodClass = methodCall.classes[i];

                RemoteType remoteType = new RemoteType(methodClass, methodInfo.GetParameters());
                if (remoteType.LocalTypeFullName == null)
                {
                    args.Add(null);
                    continue;
                }
                Type type = asm.GetType(remoteType.LocalTypeFullName);
                if (type == null)
                    type = Type.GetType(remoteType.LocalTypeFullName);
                if (type == null)
                {
                    foreach (ParameterInfo param in methodInfo.GetParameters())
                    {
                        if (param.ParameterType.FullName.ToUpper().Equals(remoteType.LocalTypeFullName.ToUpper()))
                        {
                            type = param.ParameterType;
                        }
                    }
                }

                if (type == null)
                    throw new BridgeException("no corresponding local type found");

                object obj = null;
                if (type.IsInstanceOfType(arg))
                {
                    obj = arg;
                }
                else if (type.IsPrimitive || type.Equals(typeof(string)))
                {
                    obj = arg;
                }
                else if (type.IsEnum)
                {
                    obj = Enum.Parse(type, (string)arg);
                }
                else
                {
                    obj = Marshaller.UnmarshallObject(arg.ToString(), type);
                }
                args.Add(obj);
            }
            HelpMethods.AddTrueForSpecified(args, methodInfo);
            return args.ToArray();
        }

        
        private Type findType(String typeString, MethodInfo methodInfo)
        {
            Assembly asm = typeof(T).GetType().Assembly;
            Type type = asm.GetType(typeString);
            if (type == null)
                type = Type.GetType(typeString);
            if (type == null)
            {
                foreach (ParameterInfo param in methodInfo.GetParameters())
                {
                    if (param.ParameterType.FullName.ToUpper()
                        .Equals(typeString.ToUpper()))
                        type = param.ParameterType;
                }
            }
            return type;
        }
        /// <summary>
        /// Invokes a method
        /// </summary>
        /// <param name="request">Method informations</param>
        /// <returns>return value</returns>
        protected Object InvokeMethod(IMethodCall request)
        {
            Logger.Info("Search and invoke method: " + request.methodName);
            MethodInfo methInfo = FindMethodInDomain(request);
            if (methInfo == null)
            {
                Logger.Error("No corresponding method found");
                throw new BridgeException("No corresponding method found");
            }
            Object[] arguments = CreateMethodArguments(request, methInfo);
            Object result = methInfo.Invoke(DomainService, arguments);

            Logger.Info("Invokation done");
            return result;
        }
        /// <summary>
        /// Tries to find the method that should be called.
        /// </summary>
        /// <param name="methodCall">The methodCall can be wrapped</param>
        /// <returns></returns>
        protected MethodInfo FindMethodInDomain(IMethodCall methodCall)
        {
            foreach (MethodInfo methodInfo in domainService.GetType().GetMethods())
            {
                if (methodCall.methodName.ToLower() != methodInfo.Name.ToLower())
                    continue;
                List<ParameterInfo> parameterResult = methodInfo.GetParameters().ToList<ParameterInfo>();
                if ((parameterResult.Count != methodCall.args.Count) &&
                    (HelpMethods.AddTrueForSpecified(parameterResult, methodInfo) != methodCall.args.Count))
                    continue;

                if (!HelpMethods.TypesAreEqual(methodCall.classes, parameterResult.ToArray<ParameterInfo>()))
                    continue;

                return methodInfo;
            }
            return null;
        }
        /// <summary>
        /// Converts a OpenEngsWrapper array to a String array
        /// </summary>
        /// <returns>List of strings</returns>
        #endregion
        #region Public Methods
        public DomainReverse(T domainService)
        {
            this.domainService = domainService;
        }
        /// <summary>
        /// Starts a thread which waits for messages.
        /// An exception will be thrown, if the method has already been called.
        /// </summary>
        public void Start()
        {
            if (QueueThread != null)
                throw new ApplicationException("QueueThread already started!");
            Logger.Info("Start open the Queue Thread to listen for messages from OpenEngSB.");
            IsEnabled = true;
            if (CreateService)
            {
                CreateRemoteProxy();
            }
            else
            {
                RegisterConnector(connectorId);
            }
            // start thread which waits for messages
            QueueThread = new Thread(
                new ThreadStart(Listen)
                );

            QueueThread.Start();
        }
        /// <summary>
        /// Stops the queue listening for messages and deletes the proxy on the bus.
        /// </summary>
        public void Stop()
        {
            ExceptionHandler.Stop = true;
            IsEnabled = false;
            PortIn.Close();
            Logger.Info("Connection closed");
            JmsPort.CloseAll();
        }
        #endregion
        #region Abstract Methods
        public abstract void CreateRemoteProxy();
        public abstract void DeleteRemoteProxy();
        public abstract void Listen();
        public abstract XLinkUrlBlueprint ConnectToXLink(string ToolName, String HostId, ModelToViewsTuple[] modelsToViews);
        public abstract void RegisterConnector(String connectorId);
        public abstract void UnRegisterConnector();
        public abstract void DisconnectFromXLink(String HostId);
        #endregion
    }
}