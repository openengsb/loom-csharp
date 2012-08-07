using System;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap.Parents;

namespace Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap.FaultElements
{
    class Faultstring : EndnoteElement
    {
        public Faultstring(String message) : base(message) { }
    }
}