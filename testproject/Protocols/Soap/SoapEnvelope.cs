using System;
using System.Collections.Generic;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap.Parents;

namespace Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap
{
    public class SoapEnvelope : NodeElements, IProtocol
    {
        private String[] htmlInfo;
        public int SocketNumber { get; set; }
        private String xmlversion;
        private SoapHeader header;
        private SoapBody body;
        public SoapEnvelope(String message):this(message,-1)
        {            
        }
        public SoapEnvelope(String message, int SocketNumber)
            : base(message)
        {
            int start=0;
            this.SocketNumber = SocketNumber;
            int end = message.Length;
            if (!message.Contains("<"))
            {
                htmlInfo = message.Substring(start, end).Split(new string[] { Environment.NewLine }, StringSplitOptions.None); 
                return;
            }
            end = message.IndexOf("<");
            htmlInfo = message.Substring(start, end).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            if (end <= 0)
            {
                end = 1;
            }
            String result=message.Substring(end-1,message.Length-end+1);
            result=result.Replace("\n", "").Replace("\r\n","");
            start = 0;
            end=result.IndexOf(">")+1;
            if (result.ToUpper().Replace(" ","").Contains("<?XML"))
            {
                xmlversion = result.Substring(start, end);
                result = result.Substring(end, result.Length - end);
            }

            String sbody = splittext("body", result);
            if (!String.IsNullOrEmpty(sbody))
            {
                body = new SoapBody(sbody);
            }
            String sheader = splittext("header", result);
            if (!String.IsNullOrEmpty(sheader))
            {
                header = new SoapHeader(sheader);
            }
        }
        
        public override String ToString()
        {
            List<Object> tmp = new List<Object>();
            tmp.Add(header);
            tmp.Add(body);
            String htmlresult = "";
            for (int i=0; i < htmlInfo.Length; i++)
            {
                if (String.IsNullOrEmpty(htmlInfo[i]))
                {
                    continue;
                }
                if (i + 1 < htmlInfo.Length)
                {
                    htmlresult += htmlInfo[i] + Environment.NewLine;
                }
            }
            
            htmlresult = htmlresult.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            if (!String.IsNullOrEmpty(htmlresult)) htmlresult += Environment.NewLine;
            String result = htmlresult + xmlversion + base.GetString(tmp).Replace("\r\n", "").Replace("\n", "");
            return result;
        }
        public IProtocol ConvertToProtocol(byte[] message)
        {
            ASCIIEncoding asci = new ASCIIEncoding();
            return new SoapEnvelope(asci.GetString(message),-1);
        }
        public int CompareTo(IProtocol protocol)
        {
            Boolean result = true;
            SoapEnvelope envelopr = (SoapEnvelope)protocol;
            if (envelopr.htmlInfo != null && htmlInfo != null)
            {
                if (htmlInfo.Length == 1 && !htmlInfo[0].ToUpper().Contains("IGNOREFIELD"))
                {
                    foreach (String element in envelopr.htmlInfo)
                    {
                        if (String.IsNullOrEmpty(element))
                        {
                            continue;
                        }
                        Boolean foundone = false;
                        foreach (String elementTocheck in htmlInfo)
                        {
                            foundone = element.Equals(elementTocheck);
                            if (foundone)
                            {
                                break;
                            }
                        }
                        result = result && foundone;
                        if (!result)
                        {
                            return 0;
                        }
                    }
                }
            }
            else
            {
                if (envelopr.htmlInfo == null && htmlInfo == null)
                {
                    result = true;
                }
                else
                {
                    return 0;
                }
            }
            if (result && body.Comapire(envelopr.body))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public byte[] GetMessage()
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            return encoder.GetBytes(this.ToString());
        }


        public bool Valid()
        {
            return htmlInfo != null && header!=null && body != null && base.NeedData();
        }


        public void RetrieveInfoFromReceivedMessage(IProtocol receivedMessage)
        {
            return;
        }

        public object Clone()
        {
            return new SoapEnvelope(this.ToString(),-1);
        }
    }
}
