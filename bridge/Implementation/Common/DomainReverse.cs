#region Copyright
// <copyright file="DomainReverse.cs" company="OpenEngSB">
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
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.RemoteObjects;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    public abstract class DomainReverse<domainServiceType> : IRegistration
    {
        #region Constants
        protected const string CreationConnectorType = "external-connector-proxy";
        protected const string CreationDeleteMethodName = "delete";
        protected const string CreationPort = "jms-json";
        protected const string CreationQueue = "receive";
        protected const string CreationRegistration = "proxyConnectorRegistry";
        protected const string CreationServiceId = "connectorManager";
        protected const string RegistrationMethodName = "registerConnector";
        protected const string RemoveXlinkConnectorMethodName = "disconnectFromXLink";
        protected const string UnregistrationMethodName = "unregisterConnector";
        protected const string XlinkMethodName = "connectToXLink";
        #endregion
        #region Logger
        /// <summary>
        /// Logger
        /// </summary>
        public static ILog Logger
        {
            get
            {
                return LogManager.GetLogger(typeof(domainServiceType));
            }
        }
        #endregion
        #region Private Variables
        /// <summary>
        /// ContextId that will be added to the MetaDatas. Default value is foo.
        /// </summary>
        public String ContextId { get; set; }

        /// <summary>
        /// Defines if the connector should create a new connector or if it should register a existing one
        /// </summary>
        private Boolean createService = true;
        #endregion
        #region Properties
        public String Destination
        {
            get;
            private set;
        }

        /// <summary>
        /// flag indicating if the listening thread should run
        /// </summary>
        public Boolean IsEnabled
        {
            get;
            private set;
        }

        /// <summary>
        /// The used marshaller
        /// </summary>
        public IMarshaller Marshaller
        {
            get;
            private set;
        }

        /// <summary>
        /// Username for the password
        /// </summary>
        public String Password
        {
            get;
            private set;
        }

        // IO port
        public IIncomingPort PortIn
        {
            get;
            private set;
        }

        // Thread listening for messages
        public Thread QueueThread
        {
            get;
            private set;
        }

        /// <summary>
        /// The id, which has been used to register the connector
        /// </summary>
        public Object RegisterId
        {
            get;
            protected set;
        }

        /// <summary>
        /// indicates in witch state the registration is
        /// </summary>
        public ERegistration RegistrationProcess
        {
            get;
            set;
        }

        /// <summary>
        /// Username for the authentification
        /// </summary>
        public String Username
        {
            get;
            protected set;
        }

        public string ConnectorId
        {
            get
            {
                return connectorId;
            }

            set
            {
                connectorId = value;
            }
        }

        public String DomainName
        {
            get
            {
                return domainName;
            }
        }

        public domainServiceType DomainService
        {
            get
            {
                return domainService;
            }
        }

        /// <summary>
        /// Descrips if the Registration has been done.
        /// </summary>
        public Boolean Registered
        {
            get
            {
                return RegistrationProcess.Equals(ERegistration.REGISTERED);
            }
        }

        protected ABridgeExceptionHandling ExceptionHandler
        {
            get;
            set;
        }
        #endregion
        #region Private Variables
        /// <summary>
        /// ServiceId of the proxy on the bus
        /// </summary>
        private String connectorId;

        /// <summary>
        ///  DomainType string required for OpenengSb
        /// </summary>
        private String domainName;

        /// <summary>
        /// domain-instance to act as reverse-proxy for
        /// </summary>
        private domainServiceType domainService;
        #endregion
        #region Constructors
        public DomainReverse()
        {
            this.RegistrationProcess = ERegistration.NONE;
            this.Marshaller = new JsonMarshaller();
            this.IsEnabled = true;
            this.QueueThread = null;
            this.ContextId = "foo";
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localDomainService">LocalDomain</param>
        /// <param name="host">Host</param>
        /// <param name="connectorId">ServiceId</param>
        /// <param name="domainName">name of the remote Domain</param>
        /// <param name="domainEvents">Type of the remoteDomainEvents</param>
        public DomainReverse(domainServiceType localDomainService, string host, string connectorId, string domainName, Boolean createNewConnector, ABridgeExceptionHandling exceptionhandler)
            : this()
        {
            this.ExceptionHandler = exceptionhandler;
            this.Destination = JmsDestination.CreateDestinationString(host, connectorId);
            this.connectorId = connectorId;
            this.domainName = domainName;
            this.domainService = localDomainService;
            this.PortIn = new JmsIncomingPort(Destination, exceptionhandler, connectorId);
            this.Username = "admin";
            this.Password = "password";
            this.createService = createNewConnector;
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
        public DomainReverse(domainServiceType localDomainService, string host, string serviceId, string domainName, String username, String password, Boolean createNewConnector, ABridgeExceptionHandling exceptionhandler)
            : this(localDomainService, host, serviceId, domainName, createNewConnector, exceptionhandler)
        {
            this.Username = username;
            this.Password = password;
        }
        #endregion
        #region Abstract Methods
        // Change to const when OSBB 2.4.0 is deleted        
        protected abstract string AuthentificationClass
        {
            get;
        }

        // Change to const when OSBB 2.4.0 is deleted        
        protected abstract string CreationMethodName
        {
            get;
        }

        public abstract XLinkUrlBlueprint ConnectToXLink(string toolName, String hostId, ModelToViewsTuple[] modelsToViews);

        public abstract void CreateRemoteProxy();

        public abstract void DeleteRemoteProxy();

        public abstract void DisconnectFromXLink(String hostId);

        public abstract void Listen();

        public abstract void RegisterConnector(String connectorId);
        #endregion
        #region Public Methods
        /// <summary>
        /// Starts a thread which waits for messages.
        /// An exception will be thrown, if the method has already been called.
        /// </summary>
        public void Start()
        {
            Logger.Info("Start open the Queue Thread to listen for messages from OpenEngSB.");
            IsEnabled = true;
            if (createService)
            {
                CreateRemoteProxy();
            }
            else
            {
                RegisterConnector(connectorId);
            }

            // Start thread which waits for messages
            QueueThread = new Thread(new ThreadStart(Listen));
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
            JmsPort.CloseAll(connectorId);
        }

        public abstract void UnRegisterConnector();
        #endregion
        #region Protected Methods
        /// <summary>
        /// Unmarshalls the arguments of a MethodCall.
        /// </summary>
        /// <param name="methodCall">MethodCall</param>
        /// <returns>Arguments</returns>
        protected object[] CreateMethodArguments(IMethodCall methodCall, MethodInfo methodInfo)
        {
            Type extendtypeWith = typeof(IOpenEngSBModel);
            IList<object> args = new List<object>();
            Assembly asm = typeof(domainServiceType).GetType().Assembly;
            for (int i = 0; i < methodCall.Args.Count; ++i)
            {
                Object arg = methodCall.Args[i];
                String methodClass = methodCall.Classes[i];

                RemoteType remoteType = new RemoteType(methodClass, methodInfo.GetParameters());
                if (remoteType.LocalTypeFullName == null)
                {
                    args.Add(null);
                    continue;
                }

                Type type = asm.GetType(remoteType.LocalTypeFullName);
                if (type == null)
                {
                    type = Type.GetType(remoteType.LocalTypeFullName);
                }

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
                {
                    throw new BridgeException("no corresponding local type found");
                }


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

        /// <summary>
        /// Tries to find the method that should be called.
        /// </summary>
        /// <param name="methodCall">The methodCall can be wrapped</param>
        /// <returns></returns>
        protected MethodInfo FindMethodInDomain(IMethodCall methodCall)
        {
            foreach (MethodInfo methodInfo in domainService.GetType().GetMethods())
            {
                if (methodCall.MethodName.ToLower() != methodInfo.Name.ToLower())
                {
                    continue;
                }

                List<ParameterInfo> parameterResult = methodInfo.GetParameters().ToList<ParameterInfo>();
                if ((parameterResult.Count != methodCall.Args.Count) &&
                    (HelpMethods.AddTrueForSpecified(parameterResult, methodInfo) != methodCall.Args.Count))
                {
                    continue;
                }

                if (!HelpMethods.TypesAreEqual(methodCall.Classes, parameterResult.ToArray<ParameterInfo>()))
                {
                    continue;
                }

                return methodInfo;
            }

            return null;
        }

        /// <summary>
        /// Invokes a method
        /// </summary>
        /// <param name="request">Method informations</param>
        /// <returns>return value</returns>
        protected Object InvokeMethod(IMethodCall request)
        {
            Logger.Info("Search and invoke method: " + request.MethodName);
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
        #endregion
        #region Private Methods
        /// <summary>
        /// Searchs a Type (as String) in a MethodInfo (Parses all Method names and Parameters)
        /// </summary>
        /// <param name="typeString">Type in String format</param>
        /// <param name="methodInfo">All the method to parse</param>
        /// <returns>The Type found</returns>
        private Type FindType(String typeString, MethodInfo methodInfo)
        {
            Assembly assembly = typeof(domainServiceType).GetType().Assembly;
            Type type = assembly.GetType(typeString);
            if (type == null)
            {
                type = Type.GetType(typeString);
            }

            if (type == null)
            {
                foreach (ParameterInfo param in methodInfo.GetParameters())
                {
                    if (param.ParameterType.FullName.ToUpper()
                        .Equals(typeString.ToUpper()))
                    {
                        type = param.ParameterType;
                    }
                }
            }

            return type;
        }
        #endregion
    }
}