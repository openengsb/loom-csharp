using System;
using System.Collections.Generic;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap.Parents;

namespace Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap
{
    class SoapBody : NodeElements
    {
        public String body { get; protected set; }
        public SoapFault fault { get; protected set; }

        public SoapBody(String input)
            : base(input)
        {
            String message = input;
            int start = 0;
            String result = "";
            if (!String.IsNullOrEmpty(result = base.splittext("fault", message)))
            {
                fault = new SoapFault(result);
                message = message.Replace(result, "");
            }
            start = message.IndexOf(">") + 1;
            body = message.Substring(start, message.ToUpper().LastIndexOf("<") - start);
        }

        public override String ToString()
        {
            List<Object> tmp = new List<Object>();
            tmp.Add(body);
            tmp.Add(fault);
            return base.GetString(tmp);
        }
        public Boolean Comapire(SoapBody body)
        {
            Boolean result = false;
            if (body.body != null && body != null)
            {
                result = this.body.Equals(body.body.ToString());
            }
            else
            {
                if (body.body == null && body == null)
                {
                    result = true;
                }
            }
            if (body.fault != null && fault != null)
            {
                result = result && fault.Comapire(body.fault);
            }
            else
            {
                if (body.fault == null && fault == null)
                {
                    result = result && true;
                }
            }
            return result;
        }
    }
}

