using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation
{
    public abstract class OpenEngSBImplementationSupportManager
    {
        public OpenEngSBImplementationSupportManager(Assembly assembly)
        {
            DomainFactoryProvider.addSupport(assembly);
        }
    }
}
