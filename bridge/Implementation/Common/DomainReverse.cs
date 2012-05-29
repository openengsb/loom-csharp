using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Implementation.Common.RemoteObjects;
using System.Threading;
using Implementation.Communication;
using Implementation.Communication.Json;
using Implementation.Communication.Jms;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Implementation.Common.Enumeration;

namespace Implementation.Common
{
    public abstract class DomainReverse<T>
    {
        #region Const.
        protected const string CREATION_QUEUE = "receive";
        protected const string CREATION_SERVICE_ID = "connectorManager";
        protected const string CREATION_METHOD_NAME = "create";
        protected const string CREATION_DELETE_METHOD_NAME = "delete";
        protected const string CREATION_PORT = "jms-json";
        protected const string CREATION_CONNECTOR_TYPE = "external-connector-proxy";
        protected string AUTHENTIFICATION_CLASS = "org.openengsb.connector.usernamepassword.Password";
        #endregion
        #region Variables
        /// <summary>
        /// domain-instance to act as reverse-proxy for
        /// </summary>
        private T domainService;
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
        public DomainReverse(T localDomainService, string host, string serviceId, string domainType)
        {
            this.marshaller = new JsonMarshaller();
            this.isEnabled = true;
            this.destination = Destination.CreateDestinationString(host, serviceId);
            this.queueThread = null;
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.portIn = new JmsIncomingPort(destination);
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
        public DomainReverse(T localDomainService, string host, string serviceId, string domainType, String username, String password)
        {
            this.marshaller = new JsonMarshaller();
            this.isEnabled = true;
            this.destination = Destination.CreateDestinationString(host, serviceId);
            this.queueThread = null;
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.portIn = new JmsIncomingPort(destination);
            this.username = username;
            this.password = password;
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
                object arg = methodCall.args[i];
                RemoteType remoteType = new RemoteType(methodCall.classes[i], methodInfo.GetParameters());
                if (remoteType.LocalTypeFullName == null)
                {
                    args.Add(null);
                    continue;
                }
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
                    obj = Enum.Parse(type, (string)arg);
                }
                else
                {
                    obj = marshaller.UnmarshallObject(arg.ToString(), type);
                }
                HelpMethods.addTrueForSpecified(args, methodInfo);
                args.Add(obj);
            }

            return args.ToArray();
        }
        /// <summary>
        /// Invokes a method
        /// </summary>
        /// <param name="request">Method informations</param>
        /// <returns>return value</returns>
        protected Object invokeMethod(IMethodCall request)
        {
            MethodInfo methInfo = FindMethodInDomain(request);
            if (methInfo == null)
                throw new ApplicationException("No corresponding method found");

            object[] arguments = CreateMethodArguments(request, methInfo);
            return methInfo.Invoke(DomainService, arguments); ;
        }        
        /// <summary>
        /// Tries to find the method that should be called.
        /// </summary>
        /// <param name="methodCallWrapper"></param>
        /// <returns></returns>
        protected MethodInfo FindMethodInDomain(IMethodCall methodCallWrapper)
        {
            foreach (MethodInfo methodInfo in domainService.GetType().GetMethods())
            {
                if (methodCallWrapper.methodName.ToLower() != methodInfo.Name.ToLower()) continue;
                List<ParameterInfo> parameterResult = methodInfo.GetParameters().ToList<ParameterInfo>();
                if (parameterResult.Count != methodCallWrapper.args.Count)
                {
                    if (HelpMethods.AddTrueForSpecified(parameterResult, methodInfo) != methodCallWrapper.args.Count) continue;
                }
                if (!HelpMethods.TypesAreEqual(methodCallWrapper.classes, parameterResult.ToArray<ParameterInfo>())) continue;
                return methodInfo;
            }
            return null;
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
        #region Abstract Methods
        public abstract void CreateRemoteProxy();
        public abstract void DeleteRemoteProxy();
        public abstract void Listen();
        #endregion       
    }
}