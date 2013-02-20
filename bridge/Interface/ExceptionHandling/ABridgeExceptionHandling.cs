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
        public Boolean stop
        {
            get;
            set;
        }
        public ABridgeExceptionHandling()
        {
            stop = false;
        }
        public abstract Object HandleException(Exception ex,params object[] parameters);
        public delegate Object ThrowExceptionMethod(params object[] obj);
        public event ThrowExceptionMethod Changed;
        protected Object Invoke(Object[] obj)
        {
            if (stop)
            {
                return null;
            }
            if (Changed == null)
            {
                throw new InvalidOperationException("An exception occured without a mehtod call");
            }
            return Changed.Invoke(obj);
        }
    }
}
