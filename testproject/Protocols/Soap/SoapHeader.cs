using System;
using System.Collections.Generic;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap.Parents;

namespace Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap
{
    class SoapHeader : NodeElements
    {
        private String body = null;
        public SoapHeader (String message) : base(message) {
            int start = message.IndexOf(">") + 1;
            body = message.Substring(start, message.ToUpper().LastIndexOf("<") - start);
        }

        public override String ToString()
        {
            List<Object> tmp = new List<Object>();
            tmp.Add(body);
            return base.GetString(tmp);
        }
    }
}