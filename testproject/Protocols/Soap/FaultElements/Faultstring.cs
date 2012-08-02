using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocols.Soap.Parents;

namespace Protocols.Soap.FaultElements
{
    class Faultstring : EndnoteElement
    {
        public Faultstring(String message) : base(message) { }
    }
}