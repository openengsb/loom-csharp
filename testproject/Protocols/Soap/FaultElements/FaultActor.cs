using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocols.Soap.Parents;

namespace Protocols.Soap.FaultElements
{
    class FaultActor : EndnoteElement
    {
        public FaultActor(String message) : base(message) { }
    }
}