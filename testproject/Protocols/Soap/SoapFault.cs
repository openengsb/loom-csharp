using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocols.Soap.FaultElements;
using Protocols.Soap.Parents;

namespace Protocols.Soap
{
    class SoapFault : NodeElements
    {
        public Faultcode faultcode { get; protected set; }
        public Faultstring faultstring { get; protected set; }
        public FaultActor faultactor { get; protected set; }
        public Detail detail { get; protected set; }


        public SoapFault(String input)
            : base(input)
        {
            String message = input;
            String result = "";
            if (!String.IsNullOrEmpty(result = base.splittext("faultcode", message)))
            {
                faultcode = new Faultcode(result);
                message = message.Replace(result, "");
            }
            if (!String.IsNullOrEmpty(result = base.splittext("faultactor", message)))
            {
                faultactor = new FaultActor(result);
                message = message.Replace(result, "");
            }
            if (
                !String.IsNullOrEmpty(result = base.splittext("faultstring", message)))
            {
                faultstring = new Faultstring(result);
                message = message.Replace(result, "");
            }
            if (!String.IsNullOrEmpty(result = base.splittext("detail", message)))
            {
                detail = new Detail(result);
                message = message.Replace(result, "");
            }
        }
        public override String ToString()
        {
            List<Object> tmp = new List<Object>();
            tmp.Add(faultcode);
            tmp.Add(faultstring);
            tmp.Add(faultactor);
            tmp.Add(detail);
            return base.GetString(tmp);
        }
        public Boolean Comapire(SoapFault fault)
        {
            return faultcode.Comapire(fault.faultcode) &&
                faultstring.Comapire(fault.faultstring) &&
                    faultactor.Equals(fault.faultactor) &&
                        detail.Equals(fault.detail);
        }
    }
}
