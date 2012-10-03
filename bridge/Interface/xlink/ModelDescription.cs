using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.xlink
{
    public class ModelDescription
    {
        public String modelClassName { get; set; }
        public String versionString { get; set; }
        public ModelDescription() { }
        public ModelDescription(String modelClassName, String versionString)
        {
            this.modelClassName = modelClassName;
            this.versionString = versionString;
        }

        public ModelDescription(String modelClassName)
            : this(modelClassName, "3.0.0")
        {
        }
    }
}