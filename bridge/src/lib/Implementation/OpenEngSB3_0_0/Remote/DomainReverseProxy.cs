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
using Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects;
using Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Communication.Json;
using Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Communication.Jms;
using Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Communication;
using Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Common;

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.OpenEngSB3_0_0.Remote
{
    /// <summary>
    /// This class builds reverse proxies for resources (class instances) on the
    /// client side for the bus.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DomainReverseProxy<T>: IStoppable
    {
        #region Const.
        private const string _CREATION_QUEUE = "receive";
        private const string CREATION_SERVICE_ID = "connectorManager";
        private const string _CREATION_METHOD_NAME = "create";
        private const string _CREATION_DELETE_METHOD_NAME = "delete";
        private const string _CREATION_PORT = "jms-json";
        private const string _CREATION_CONNECTOR_TYPE = "external-connector-proxy";
        #endregion
        #region Variabels
        /// <summary>
        /// Username for the authentification
        /// </summary>
        private String username;
        /// <summary>
        /// Username for the password
        /// </summary>
        private String password;
        // Thread listening for messages
        private Thread queueThread;

        // IO port
        private IIncomingPort portIn;

        private string destination;

        /// <summary>
        /// ServiceId of the proxy on the bus
        /// </summary>
        private string serviceId;

        /// <summary>
        ///  DomainType string required for OpenengSb
        /// </summary>
        private string domainType;

        /// <summary>
        /// domain-instance to act as reverse-proxy for
        /// </summary>
        private T domainService;

        /// <summary>
        /// flag indicating if the listening thread should run
        /// </summary>

        private bool isEnabled;

        private IMarshaller marshaller;

        /// <summary>
        /// Identifies the service-instance.
        /// </summary>
        private ConnectorId connectorId;
        #endregion
        #region Propreties
        public T DomainService
        {
            get { return domainService; }
        }
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
        public DomainReverseProxy(T localDomainService, string host, string serviceId, string domainType)
        {
            this.marshaller = new JsonMarshaller();
            this.isEnabled = true;            
            this.domainService = localDomainService;
            this.destination = Destination.CreateDestinationString(host, serviceId);
            this.queueThread = null;
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.portIn = new JmsIncomingPort(destination);
            this.connectorId = null;
            this.username = "admin";
            this.password = "password";
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
        public DomainReverseProxy(T localDomainService, string host, string serviceId, string domainType,String username,String password)
        {
            this.marshaller = new JsonMarshaller();
            this.isEnabled = true;
            this.domainService = localDomainService;
            this.destination = Destination.CreateDestinationString(host, serviceId);
            this.queueThread = null;
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.portIn = new JmsIncomingPort(destination);
            this.connectorId = null;
            this.username = username;
            this.password = password;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Starts a thread which waits for messages.
        /// An exception will be thrown, if the method has already been called.
        /// </summary>
        public void Start()
        {
            if (queueThread != null)
                throw new ApplicationException("QueueThread already started!");

            isEnabled = true;
            CreateRemoteProxy();
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
                DeleteRemoteProxy();
            }
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Creates an Proxy on the bus.
        /// </summary>
        private void CreateRemoteProxy()
        {
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);
            Guid id = Guid.NewGuid();

            String classname = "org.openengsb.connector.usernamepassword.Password";

            IList<string> classes = new List<string>();
            classes.Add("org.openengsb.core.api.model.ConnectorId");
            classes.Add("org.openengsb.core.api.model.ConnectorDescription");

            IList<object> args = new List<object>();
            ConnectorDescription connectorDescription = new ConnectorDescription();
            connectorDescription.attributes.Add("serviceId", serviceId);
            connectorDescription.attributes.Add("portId", _CREATION_PORT);
            connectorDescription.attributes.Add("destination", destination);

            connectorId = new ConnectorId();
            connectorId.connectorType = _CREATION_CONNECTOR_TYPE;
            connectorId.instanceId = serviceId;
            connectorId.domainType = domainType;

            args.Add(connectorId);
            args.Add(connectorDescription);

            RemoteMethodCall creationCall = RemoteMethodCall.CreateInstance(_CREATION_METHOD_NAME, args, metaData, classes,null);
            Message message = Message.createInstance(creationCall, id.ToString(), true, "");

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = _CREATION_QUEUE;
            
            Data data=Data.CreateInstance(password);
            AuthenticationInfo autinfo=AuthenticationInfo.createInstance(classname,data);
            SecureMethodCallRequest secureRequest = SecureMethodCallRequest.createInstance(username, autinfo, message);
            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination);
            string request = marshaller.MarshallObject(secureRequest);
            portOut.Send(request);         
        }

        /// <summary>
        /// Deletes the created remote proxy on the bus.
        /// </summary>
        private void DeleteRemoteProxy()
        {
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("serviceId", CREATION_SERVICE_ID);

            IList<string> classes = new List<string>();
            classes.Add("org.openengsb.core.api.model.ConnectorId");

            IList<object> args = new List<object>();
            args.Add(connectorId);

            RemoteMethodCall deletionCall = RemoteMethodCall.CreateInstance(_CREATION_DELETE_METHOD_NAME, args, metaData, classes,null);

            Guid id = Guid.NewGuid();
            String classname = "org.openengsb.connector.usernamepassword.Password";
            Data data = Data.CreateInstance(password);
            AuthenticationInfo authentification = AuthenticationInfo.createInstance(classname, data);

            Message message = Message.createInstance(deletionCall, id.ToString(), true, "");
            SecureMethodCallRequest callRequest = SecureMethodCallRequest.createInstance(username,authentification,message);

            Destination destinationinfo = new Destination(destination);
            destinationinfo.Queue = _CREATION_QUEUE;

            IOutgoingPort portOut = new JmsOutgoingPort(destinationinfo.FullDestination);
            string request = marshaller.MarshallObject(callRequest);
            portOut.Send(request);

            IIncomingPort portIn = new JmsIncomingPort(Destination.CreateDestinationString(destinationinfo.Host, callRequest.message.callId));
            string reply = portIn.Receive();

            MethodResultMessage result = marshaller.UnmarshallObject(reply, typeof(MethodResultMessage)) as MethodResultMessage;
            if (result.message.result.type == MethodResult.ReturnType.Exception)
                throw new ApplicationException("Remote Exception while deleting service proxy");            
        }

        /// <summary>
        /// Blocks an waits for messages.
        /// </summary>
        private void Listen()
        {
            while (isEnabled)
            {
                String textMsg = portIn.Receive();

                if (textMsg == null)
                    continue;
                SecureMethodCallRequest methodCallRequest = marshaller.UnmarshallObject(textMsg, typeof(SecureMethodCallRequest)) as SecureMethodCallRequest;

                MethodResultMessage methodReturnMessage = CallMethod(methodCallRequest);

                if (methodCallRequest.message.answer)
                {
                    string returnMsg = marshaller.MarshallObject(methodReturnMessage);
                    Destination dest = new Destination(destination);
                    IOutgoingPort portOut = new JmsOutgoingPort(Destination.CreateDestinationString(dest.Host, methodCallRequest.message.callId));
                    portOut.Send(returnMsg);
                }
            }
        }

        /// <summary>
        /// Calls a method according to MethodCall.
        /// </summary>
        /// <param name="methodCall">Description of the call.</param>
        /// <returns></returns>
        private MethodResultMessage CallMethod(SecureMethodCallRequest request)
        {
            MethodInfo methInfo = FindMethodInDomain(request.message.methodCall);
            if (methInfo == null)
                throw new ApplicationException("No corresponding method found");

            object[] arguments = CreateMethodArguments(request.message.methodCall);

            object returnValue = null;
            try
            {
                returnValue = methInfo.Invoke(domainService, arguments);
            }
            catch (Exception ex)
            {
                return CreateMethodReturn(MethodResult.ReturnType.Exception, ex, request.message.callId);
            }

            MethodResultMessage returnMsg = null;

            if (returnValue == null)
                returnMsg = CreateMethodReturn(MethodResult.ReturnType.Void, "null", request.message.callId);
            else
                returnMsg = CreateMethodReturn(MethodResult.ReturnType.Object, returnValue, request.message.callId);

            return returnMsg;
        }

        /// <summary>
        /// Builds an return message.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="returnValue"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        private MethodResultMessage CreateMethodReturn(MethodResult.ReturnType type, object returnValue, string correlationId)
        {
            MethodResult methodResult = new MethodResult();
            methodResult.type = type;
            methodResult.arg = returnValue;
            MethodResultMessage methodResultMessage = new MethodResultMessage();
            methodResultMessage.message=new MessageResult();
            methodResultMessage.message.callId = correlationId;

            if (returnValue == null)
                methodResult.className = "null";
            else
                methodResult.className = new LocalType(returnValue.GetType()).RemoteTypeFullName;

            methodResult.metaData = new Dictionary<string, string>();

            methodResultMessage.message.result = methodResult;
            return methodResultMessage;
        }

        /// <summary>
        /// Unmarshalls the arguments of a MethodCall.
        /// </summary>
        /// <param name="methodCall">MethodCall</param>
        /// <returns>Arguments</returns>
        private object[] CreateMethodArguments(RemoteMethodCall methodCall)
        {
            IList<object> args = new List<object>();

            Assembly asm = typeof(T).GetType().Assembly;
            for (int i = 0; i < methodCall.args.Count; ++i)
            {
                object arg = methodCall.args[i];
                RemoteType remoteType = new RemoteType(methodCall.classes[i]);
                Type type = asm.GetType(remoteType.LocalTypeFullName);
                
                if (type == null)
                    type = Type.GetType(remoteType.LocalTypeFullName);

                if (type == null)
                    throw new ApplicationException("no corresponding local type found");

                object obj = null;
                if (type.IsPrimitive || type.Equals(typeof(string)))
                {
                    obj = arg;
                }
                else if (type.IsEnum)
                {
                    obj = Enum.Parse(type,(string) arg);
                }
                else
                {
                    obj = marshaller.UnmarshallObject(arg.ToString(), type);
                }
                args.Add(obj);
            }

            return args.ToArray();
        }

        /// <summary>
        /// Tries to find the method that should be called.
        ///  TODO remove " if (methodCallWrapper.args.Count > methodCallWrapper.classes.Count)" if the Bug OPENENGSB-2423/OPENENGSB-2429 is fixed
        /// </summary>
        /// <param name="methodCallWrapper"></param>
        /// <returns></returns>
        private MethodInfo FindMethodInDomain(RemoteMethodCall methodCallWrapper)
        {
            if (methodCallWrapper.args.Count > methodCallWrapper.classes.Count)
            {
                int tmp=methodCallWrapper.args.Count - methodCallWrapper.classes.Count;
                int i;
                Object[] nullObject = new object[1];
                String nullObjectString = nullObject.GetType().ToString();
                for (i = 0;i<tmp; i++)
                {
                    methodCallWrapper.classes.Add(nullObject.GetType().ToString());
                }
            }

            foreach (MethodInfo methodInfo in domainService.GetType().GetMethods())
            {
                if (methodCallWrapper.methodName.ToLower() != methodInfo.Name.ToLower())
                {
                    continue;
                }

                if (methodInfo.GetParameters().Length != methodCallWrapper.args.Count)
                {
                    continue;
                }
                if (!TypesAreEqual(methodCallWrapper.classes, methodInfo.GetParameters()))
                {
                    continue;
                }

                return methodInfo;
            }

            return null;
        }

        /// <summary>
        /// Tests if the list of type names are equal to the types of the method parameter.
        /// </summary>
        /// <param name="typeStrings"></param>
        /// <param name="parameterInfos"></param>
        /// <returns></returns>
        private bool TypesAreEqual(IList<string> typeStrings, ParameterInfo[] parameterInfos)
        {
            if (typeStrings.Count != parameterInfos.Length)
                throw new ApplicationException("length of type-string-arrays are not equal");

            for (int i = 0; i < parameterInfos.Length; ++i)
            {
                if (!TypeIsEqual(typeStrings[i], parameterInfos[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Test if two types are equal
        /// TODO remove "null" if the Bug OPENENGSB-2423/OPENENGSB-2429 is fixed
        /// </summary>
        /// <param name="remoteType">Remote Type</param>
        /// <param name="localType">Local Type</param>
        /// <returns>If to types are equal</returns>
        private bool TypeIsEqual(string remoteType, Type localType)
        {
            if (remoteType.Equals("null")) return true;
            RemoteType remote_typ = new RemoteType(remoteType);
            // leading underscore fix
            return (remote_typ.LocalTypeFullName == localType.FullName);
        }
        #endregion
    }
}
