/***
 * Licensed to the Austrian Association for Software Tool Integration (AASTI)
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. The AASTI licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and

 * limitations under the License.
 ***/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Bridge.Implementation
{
    public class HelpMethods
    {
        /// <summary>
        /// Takes a Namespace as input, reverse the elements and returns the package structure from java
        /// </summary>
        /// <param name="url">Namespace URL</param>
        /// <returns>Java Package structure</returns>
        public static String reverseURL(String url)
        {
            String tmp = url.Replace("http://", "");
            tmp = tmp.Substring(0, tmp.IndexOf("/"));
            tmp = tmp.Replace("/", "");
            String[] elements = tmp.Split('.');
            int i;
            String result = "";
            for (i = elements.Length - 1; i >= 0; i--)
            {
                if (i != 0) result += elements[i] + ".";
                else result += elements[i];
            }
            return result;
        }

        /// <summary>
        /// Search in the interface for the Namespace (equal to the package structure in java)
        /// </summary>
        /// <param name="fieldname">Method name or Parameter name</param>
        /// <returns>Packagename</returns>
        public static String GetPackageName(String fieldname,Type type)
        {
            MethodInfo method = type.GetMethod(fieldname);
            //Tests if it is a Mehtod or a Type
            if (method != null)
            {
                SoapDocumentMethodAttribute soapAttribute;
                foreach (Attribute attribute in method.GetCustomAttributes(false))
                {
                    if (attribute is SoapDocumentMethodAttribute)
                    {
                        soapAttribute = attribute as SoapDocumentMethodAttribute;
                        return reverseURL(soapAttribute.RequestNamespace);
                    }
                }
            }
            else
            {
                Assembly ass = type.Assembly;
                type = ass.GetType(fieldname);
                foreach (Attribute attribute in Attribute.GetCustomAttributes(type))
                {
                    if (attribute is XmlTypeAttribute)
                    {
                        XmlTypeAttribute xmltype = attribute as XmlTypeAttribute;
                        return reverseURL(xmltype.Namespace);
                    }
                }
            }
            throw new MethodAccessException("Fieldname doesn't have a corresponding attribute (Namepspace) or the attribute couldn't be found");
        }
        /// <summary>
        /// Makes the first character to a upper character
        /// </summary>
        /// <param name="element">Element to edit</param>
        /// <returns>String with the first character upper</returns>
        public static String FirstLetterToUpper(String element)
        {
            if (element.Length <= 1) return element.ToUpper();
            String first = element.Substring(0, 1);
            first = first.ToUpper();
            String tmp = element.Substring(1);
            return first + tmp;
        }
    }
}
