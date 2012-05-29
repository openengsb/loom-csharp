using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Implementation.Common.RemoteObjects
{
    public interface IMethodCall
    {
        /// <summary>
        /// Fully qualified class names of the arguments.
        /// </summary>
        IList<string> classes { get; set; }
        /// <summary>
        /// Name of the method to be called.
        /// </summary>
        string methodName { get; set; }

        /// <summary>
        /// Arguments of the call.
        /// </summary>
        IList<object> args { get; set; }
    }
}
