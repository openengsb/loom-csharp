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
using Implementation.Common.RemoteObjects;
using Implementation.Communication;
using Implementation.Communication.Jms;
using Implementation.Communication.Json;
using log4net;
using Implementation.Common.Enumeration;
using Implementation.Exceptions;

namespace Implementation.Common
{
    public abstract class DomainReverse<T> : IRegistration
    {
        #region Const.
        protected const string CREATION_QUEUE = "receive";
        protected const string CREATION_SERVICE_ID = "connectorManager";
        protected const string CREATION_REGISTRATION = "proxyConnectorRegistry";
        protected const string CREATION_DELETE_METHOD_NAME = "delete";
        protected const string CREATION_PORT = "jms-json";
        protected const string CREATION_CONNECTOR_TYPE = "external-connector-proxy";
        protected const string REGISTRATION_METHOD_NAME = "registerConnector";
        protected const string UNREGISTRATION_METHOD_NAME = "unregisterConnector";
        #endregion
        #region Propreties
        protected abstract string CREATION_METHOD_NAME { get; }
        protected abstract string AUTHENTIFICATION_CLASS { get; }
        public string ServiceID { get { return serviceId; } }
        #endregion
        #region Variables
        /// <summary>
        /// indicates in witch state the registration is
        /// </summary>
        protected ERegistration registrationprocess = ERegistration.NONE;
        /// <summary>
        ///Descrips if the Registration has been done.
        /// </summary>
        public Boolean Registered
        {
            get { return registrationprocess.Equals(ERegistration.REGISTERED); }
        }
        /// <summary>
        /// Defines if the connector should create a new connector or if it should register a existing one
        /// </summary>
        protected Boolean createService = true;

        /// <summary>
        /// domain-instance to act as reverse-proxy for
        /// </summary>
        private T domainService;
        /// <summary>
        /// Logger
        /// </summary>
        protected static ILog logger = LogManager.GetLogger(typeof(T));
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
        protected String username;
        /// <summary>
        /// Username for the password
        /// </summary>
        protected String password;
        // Thread listening for messages
        protected Thread queueThread;

        // IO port
        protected IIncomingPort portIn;

        protected String destination;

        /// <summary>
        /// ServiceId of the proxy on the bus
        /// </summary>
        protected String serviceId;

        /// <summary>
        ///  DomainType string required for OpenengSb
        /// </summary>
        protected String domainType;

        /// <summary>
        /// flag indicating if the listening thread should run
        /// </summary>
        protected Boolean isEnabled;
        /// <summary>
        /// The used marshaller
        /// </summary>
        protected IMarshaller marshaller;
        /// <summary>
        /// The id, which has been used to register the connector
        /// </summary>
        protected Object registerId;
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localDomainService">LocalDomain</param>
        /// <param name="host">Host</param>
        /// <param name="serviceId">ServiceId</param>
        /// <param name="domainType">name of the remote Domain</param>
        /// <param name="domainEvents">Type of the remoteDomainEvents</param>
        public DomainReverse(T localDomainService, string host, string serviceId, string domainType, Boolean createNewConnector)
        {
            this.marshaller = new JsonMarshaller();
            this.isEnabled = true;
            this.destination = Destination.CreateDestinationString(host, serviceId);
            this.queueThread = null;
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.domainService = localDomainService;
            this.portIn = new JmsIncomingPort(destination);
            this.username = "admin";
            this.password = "password";
            this.createService = createNewConnector;
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
        public DomainReverse(T localDomainService, string host, string serviceId, string domainType, String username, String password, Boolean createNewConnector)
        {
            this.marshaller = new JsonMarshaller();
            this.isEnabled = true;
            this.destination = Destination.CreateDestinationString(host, serviceId);
            this.queueThread = null;
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.domainService = localDomainService;
            this.portIn = new JmsIncomingPort(destination);
            this.username = username;
            this.password = password;
            this.createService = createNewConnector;
        }
        #endregion
        #region Protected Methods
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
                if (methodCall.isWrapped())
                {
                    methodClass = ((OpenEngSBModelWrapper)arg).modelClass;
                    arg = ConvertWrapperTypes(arg, methodInfo);
                }

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
                    obj = marshaller.UnmarshallObject(arg.ToString(), type);
                }
                args.Add(obj);
            }
            HelpMethods.addTrueForSpecified(args, methodInfo);
            return args.ToArray();
        }
        private Object ConvertWrapperTypes(Object wrappedObject, MethodInfo methodInfo)
        {
            OpenEngSBModelWrapper wrapper = wrappedObject as OpenEngSBModelWrapper;
            IList<Object> result = new List<Object>();
            RemoteType mt = new RemoteType(wrapper.modelClass, methodInfo.GetParameters());
            Type usedType = findType(mt.LocalTypeFullName, methodInfo);
            Object obj = Activator.CreateInstance(usedType);
            foreach (OpenEngSBModelEntry entry in wrapper.entries)
            {
                PropertyInfo field = null;
                foreach (PropertyInfo info in usedType.GetProperties())
                {
                    if (info.Name.ToUpper().Equals(entry.key.ToUpper()))
                    {
                        field = info;
                        break;
                    }
                }
                if (field == null)
                    throw new ArgumentException("There is no field " + entry.key);
                Object tmp = ConvertType(entry, methodInfo);
                field.SetValue(obj, tmp, null);
            }
            return obj;
        }

        private Object ConvertType(OpenEngSBModelEntry entry, MethodInfo methodinfo)
        {
            String value = entry.value;
            RemoteType remote = new RemoteType(entry.type, methodinfo.GetParameters());
            Type type = findType(remote.LocalTypeFullName, methodinfo);
            if (type.IsPrimitive || type.Equals(typeof(string)))
            {
                return Convert.ChangeType(value, type);
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }
            else
            {
                return marshaller.UnmarshallObject(value, type);
            }
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
        protected Object invokeMethod(IMethodCall request)
        {
            logger.Info("Search and invoke method: " + request.methodName);
            MethodInfo methInfo = FindMethodInDomain(request);
            if (methInfo == null)
            {
                logger.Error("No corresponding method found");
                throw new BridgeException("No corresponding method found");
            }
            Object[] arguments = CreateMethodArguments(request, methInfo);
            Object result = methInfo.Invoke(DomainService, arguments);
            
            logger.Info("Invokation done");
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
                    (HelpMethods.addTrueForSpecified(parameterResult, methodInfo) != methodCall.args.Count))
                    continue;
                if (!methodCall.isWrapped())
                {
                    if (!HelpMethods.TypesAreEqual(methodCall.classes, parameterResult.ToArray<ParameterInfo>()))
                        continue;
                }
                else
                {
                    if (!HelpMethods.TypesAreEqual(convertToClassList(methodCall.args), parameterResult.ToArray<ParameterInfo>()))
                        continue;
                }
                return methodInfo;
            }
            return null;
        }
        /// <summary>
        /// Converts a OpenEngsWrapper array to a String array
        /// </summary>
        /// <returns>List of strings</returns>
        private IList<String> convertToClassList(IList<Object> wrappers)
        {
            IList<String> result = new List<String>();
            foreach (OpenEngSBModelWrapper wrapper in wrappers)
            {
                result.Add(wrapper.modelClass);
            }
            return result;
        }
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
            if (queueThread != null)
                throw new ApplicationException("QueueThread already started!");
            logger.Info("Start open the Queue Thread to listen for messages from OpenEngSB.");
            isEnabled = true;
            if (createService)
            {
                CreateRemoteProxy();
            }
            else
            {
                RegisterConnector(serviceId);
            }
            // start thread which waits for messages
            queueThread = new Thread(
                new ThreadStart(Listen)
                );

            queueThread.Start();
        }
        /// <summary>
        /// Stops the queue listening for messages and deletes the proxy on the bus.
        /// </summary>
        public void Stop()
        {
            if (queueThread != null)
            {
                isEnabled = false;
                portIn.Close();
            }
            logger.Info("Connection closed");
        }
        #endregion
        #region Abstract Methods
        public abstract void CreateRemoteProxy();
        public abstract void DeleteRemoteProxy();
        public abstract void Listen();
        public abstract void RegisterConnector(String serviceId);
        public abstract void UnRegisterConnector();
        #endregion
    }
}