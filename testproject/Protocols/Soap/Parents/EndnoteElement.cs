using System;
using System.Collections.Generic;

namespace Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap.Parents
{
    class EndnoteElement : NodeElements
    {
        public String Value { get; set; }

        public EndnoteElement(String message)
            : base(message)
        {
            Value = base.getValue(message);
        }

        public override String ToString()
        {
            return base.GetString(new List<object>() { Value });
        }
        public Boolean Comapire(EndnoteElement node)
        {
            return Value.Equals(node.Value);
        }
    }
}
