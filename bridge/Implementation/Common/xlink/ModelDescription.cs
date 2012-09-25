using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.xlink
{
    public class ModelDescription
    {
        private String modelClassName;
        private String versionString;
        public ModelDescription() { }
        public ModelDescription(String modelClassName, Version version)
        {
            this.modelClassName = modelClassName;
            this.versionString = version.ToString();
        }

        public ModelDescription(String modelClassName)
            : this(modelClassName, new Version(1, 0, 0))
        {
        }
    }
}