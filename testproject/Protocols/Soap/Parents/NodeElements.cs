using System;
using System.Collections.Generic;
using System.Linq;

namespace Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap.Parents
{
    public class NodeElements : Namespaces
    {
        private String prefix;
        private String name;
        public NodeElements() { }
        public NodeElements(String input)
        {
            if (!input.Contains("<"))
            {
                return;
            }
            String message = input;
            int start = 0;
            if (message.Replace(" ", "").ToUpper().Contains("<?XML"))
            {
                start = message.IndexOf(">") + 1;
                message = message.Substring(start, message.Length - start);
                start = 1;
            }
            else
            {
                start = message.IndexOf("<");
                message = message.Substring(start, message.Length - start);
            }
            start = 1;
            base.addNewElements(message);
            String line = message.Substring(start, message.IndexOf(">") - start);
            String[] elements = line.Split(' ');
            String xmls = elements[0];
            name = xmls;
            if (elements[0].Contains(':'))
            {
                elements = xmls.Split(':');
                name = elements[1];
                prefix = elements[0];
            }
        }

        public NodeElements(String name, String prefix)
        {
            this.name = name;
            this.prefix = prefix;
        }

        public String GetString(List<Object> elements)
        {
            String result = "<";
            if (!String.IsNullOrEmpty(prefix))
            {
                result += prefix + ":";
            }
            result += name;
            if (base.dictSet())
            {
                result += " " + base.ToString();
            }
            result += ">";
            foreach (Object element in elements) result += printObject(element);
            result += "</";
            if (!String.IsNullOrEmpty(prefix))
            {
                result += prefix + ":";
            }
            result += name + ">";
            return result;
        }
        public String printObject(Object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return "";
            }
        }
        public String splittext(String elementName, String message)
        {
            String name = elementName.ToUpper();
            int start = message.ToUpper().IndexOf(name);
            if (start <= 0)
            {
                return "";
            }
            String tmpMessage = message.Substring(0, start);
            start = tmpMessage.LastIndexOf("<");
            int end = message.ToUpper().LastIndexOf(name);
            tmpMessage = message.Substring(end, message.Length - end);
            end = end + tmpMessage.IndexOf(">") + 1;
            String result = message.Substring(start, end - start);
            String testelement = result + " ";
            int tmpend = testelement.IndexOf("\"");
            end = testelement.IndexOf(" ");
            if (tmpend < end && tmpend > 0)
            {
                end = tmpend;
            }
            testelement = testelement.Substring(0, testelement.IndexOf(" "));
            if (testelement.ToUpper().Contains(elementName.ToUpper()))
            {
                return result;
            }
            else
            {
                return "";
            }
        }
        public Boolean NeedData()
        {
            return String.IsNullOrEmpty(name);
        }
    }
}