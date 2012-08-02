using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocols.Soap.Parents;

namespace Protocols.Soap.FaultElements
{
    class Faultcode : EndnoteElement
    {
        public Faultcode(String message) : base(message) { }
    }
}
