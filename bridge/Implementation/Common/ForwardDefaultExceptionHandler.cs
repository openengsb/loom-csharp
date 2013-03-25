using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    public class ForwardDefaultExceptionHandler : ABridgeExceptionHandling
    {

        /// <summary>
        /// Directly forwards the exception to user user of the .Net Bridge
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Object HandleException(Exception exception, params Object[] obj)
        {
            throw exception;
        }
    }
}