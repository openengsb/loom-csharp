using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300
{
    public class OpenEngSB300 : OpenEngSBImplementationSupportManager
    {
        /// <summary>
        /// Adds this Assembly to the supportedLists
        /// </summary>
        public OpenEngSB300()
            : base(Assembly.GetExecutingAssembly())
        {
        }
        public static void SetSupport()
        {
            new OpenEngSB300();
        }
    }
}