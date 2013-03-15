using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;

namespace Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling
{
    public abstract class ABridgeExceptionHandling
    {
        /// <summary>
        /// When the stop method has been invoked. The ExceptionHandler should stop.
        /// </summary>
        public Boolean Stop
        {
            get;
            set;
        }
        public ABridgeExceptionHandling()
        {
            Stop = false;
        }
        public abstract Object HandleException(Exception exception, params object[] parameters);
        public delegate Object ThrowExceptionMethod(params object[] obj);
        public event ThrowExceptionMethod Changed;
        /// <summary>
        /// Invokes the method, that triggered the exception again
        /// </summary>
        /// <param name="obj">(Failed) Method parameter</param>
        /// <returns>Method result</returns>
        protected Object Invoke(Object[] obj)
        {
            if (Stop)
            {
                throw new InvalidOperationException("The Exceptionhandler has been stopped manually");
            }
            if (Changed == null)
            {
                throw new InvalidOperationException("An exception occured without a mehtod call");
            }
            return Changed.Invoke(obj);
        }
    }
}
