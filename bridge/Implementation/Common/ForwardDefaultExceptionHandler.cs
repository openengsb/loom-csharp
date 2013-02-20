using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    public class ForwardDefaultExceptionHandler : ABridgeExceptionHandling
    {

        public override Object HandleException(Exception exception, params Object[] obj)
        {
            throw exception;
        }
    }
}
