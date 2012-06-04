using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Implementation.Communication;
using System.Runtime.Remoting.Proxies;
using Implementation.Communication.Json;
using System.Runtime.Remoting.Messaging;
using Implementation.Common.RemoteObjects;
using Implementation.Common.Enumeration;
using log4net;

namespace Implementation.Common
{
    public abstract class Domain<T> : RealProxy
    { 
        #region Const.
        /// <summary>
        /// Name of the queue the server listens to for calls.
        /// </summary>
        protected const string HOST_QUEUE = "receive";
        protected static ILog logger = LogManager.GetLogger(typeof(T));
        #endregion
        #region Variables
        /// <summary>
        /// Authenifaction class
        /// </summary>
        protected string AUTHENTIFICATION_CLASS = "org.openengsb.core.api.security.model.UsernamePasswordAuthenticationInfo";
        /// <summary>
        /// Username for the authentification
        /// </summary>
        protected String username;
        /// <summary>
        /// Password for the authentification
        /// </summary>
        protected String password;
        /// <summary>
        /// Id identifying the service instance on the bus.
        /// </summary>
        protected String serviceId;
        /// <summary>
        /// Domain type
        /// </summary>
        protected String domainType;

        /// <summary>
        /// Host string of the server.
        /// </summary>
        protected string host;

        protected IMarshaller marshaller;
        #endregion
        #region Constructors
        public Domain(string host, string serviceId, String domainType)
            : base(typeof(T))
        {
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.host = host;
            this.marshaller = new JsonMarshaller();
            this.username = "admin";
            this.password = "password";
        }
        public Domain(string host, string serviceId, String domainType, String username, String password)
            : base(typeof(T))
        {
            this.serviceId = serviceId;
            this.domainType = domainType;
            this.host = host; ;
            this.marshaller = new JsonMarshaller();
            this.username = username;
            this.password = password;
        }
        #endregion
        #region Public Methods
        public new T GetTransparentProxy()
        {
            return (T)base.GetTransparentProxy();
        }
        #endregion
        #region Protected Methods
        /// <summary>
        /// Builds an IMessage using MethodReturn.
        /// </summary>
        /// <param name="methodReturn">Servers return message</param>
        /// <param name="callMessage">Method an parameters</param>
        /// <returns>The result of the Message</returns>
        protected IMessage ToMessage(IMethodResult methodReturn, IMethodCallMessage callMessage)
        {
            logger.Info("Convert method call to String method and send it to the OpenEngSB");
            IMethodReturnMessage returnMessage = null;
            switch (methodReturn.type)
            {
                case ReturnType.Exception:
                    returnMessage = new ReturnMessage(new Exception(methodReturn.arg + "\n" + methodReturn.ToString()), callMessage);
                    break;
                case ReturnType.Void:
                case ReturnType.Object:
                    returnMessage = new ReturnMessage(methodReturn.arg, null, 0, null, callMessage);
                    break;
                default:
                    return null;
            }
            return returnMessage;
        }
        #endregion
    }
}