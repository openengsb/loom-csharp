using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Implementation.Common.Enumeration;

namespace Implementation.Common.RemoteObjects
{    
    public interface IMethodResult
    {       
        /// <summary>
        /// Type of the return value.
        /// </summary>
        ReturnType type { get; set; }
        /// <summary>
        /// Return value of the RPC.
        /// </summary>
        object arg { get; set; }
    }
}
