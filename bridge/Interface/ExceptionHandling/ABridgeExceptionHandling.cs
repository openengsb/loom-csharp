using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;

namespace Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling
{
    public abstract class ABridgeExceptionHandling : ExceptionHandler
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
        public delegate void ThrowExceptionMethod();
        public event ThrowExceptionMethod Changed;
        protected void Invoke()
        {
            if (stop)
            {
                return;
            }
            if (Changed == null)
            {
                throw new InvalidOperationException("An exception occured without a mehtod call");
            }
            Changed.Invoke();
        }
    }
}
