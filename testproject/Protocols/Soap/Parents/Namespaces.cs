using System;
using System.Collections.Generic;

namespace Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap.Parents
{
    public class Namespaces
    {
        private Dictionary<String, Uri> namespaces = new Dictionary<String, Uri>();
        public void addNewElements(String node)
        {
            String line = node.Substring(0, node.IndexOf(">"));
            String[] elements = line.Split(' ');
            for (int i = 1; i < elements.Length; i++)
            {
                String[] tmp = elements[i].Split('=');
                String xmls = tmp[0];
                String url = tmp[1];

                namespaces.Add(xmls, new Uri(url.Replace("\"", "")));
            }

        }
        public Boolean dictSet()
        {
            return namespaces.Keys.Count > 0;
        }
        public override String ToString()
        {
            String result = null;

            foreach (String name in namespaces.Keys)
            {
                result += name + "=" + "\"" + namespaces[name] + "\" ";
            }
            if (!String.IsNullOrEmpty(result))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }
        public String getValue(String message)
        {
            int start = message.IndexOf(">") + 1;
            String tmpString = message.Substring(start, message.Length - start);
            return tmpString.Substring(0, tmpString.LastIndexOf("<"));
        }
    }
}
