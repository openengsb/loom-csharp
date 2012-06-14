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
using Implementation.Common;
using System.Web.Services;
using Implementation.Common.RemoteObjects;
using Implementation.Communication.Json;

namespace Implementation
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
        private static String FindCXFRequestNamespace(Type type, String fieldname)
        {
            List<String> elements = new List<string>();            
            String name = fieldname;
            if (name.Contains('.'))
            {
                int start = name.LastIndexOf('.')+1;
                name = name.Substring(start, name.Length - start);
            }
            foreach (MethodInfo method in type.GetMethods())
            {
                foreach (Attribute attribute in method.GetCustomAttributes(false))
                {
                    if (attribute is SoapDocumentMethodAttribute)
                    {
                        SoapDocumentMethodAttribute soapAttribute = attribute as SoapDocumentMethodAttribute;
                        String[] result = soapAttribute.RequestNamespace.Split(';');
                        foreach (String s in result)
                        {
                            String[] element = s.Split(':');
                            if (element.Length>1 && element[0].ToUpper().Equals(name.ToUpper())) return element[1];
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Search in the interface for the Namespace (equal to the package structure in java)
        /// </summary>
        /// <param name="fieldname">Method name or Parameter name</param>
        /// <returns>Packagename</returns>
        public static String GetPackageName(String fieldname, Type type)
        {
            SoapDocumentMethodAttribute soapAttribute;            
            foreach (Attribute attribute in type.GetCustomAttributes(false))
            {
                if (attribute is WebServiceBindingAttribute)
                {
                    WebServiceBindingAttribute webservice = attribute as WebServiceBindingAttribute;
                    String result = FindCXFRequestNamespace(type, fieldname);
                    if (!String.IsNullOrEmpty(result)) return result;
                }
            }

            MethodInfo method = type.GetMethod(fieldname);
            //Tests if it is a Mehtod or a Type
            if (method != null)
            {                
                
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
        /// <summary>
        /// Add true objects for the Specified fields
        /// </summary>
        /// <param name="args">List of parameters for a methodcall</param>
        /// <param name="m">Methodinfo</param>
        public static void addTrueForSpecified(IList<object> args, MethodInfo m)
        {
            ParameterInfo[] paraminfo = m.GetParameters();
            if (paraminfo.Length <= args.Count && paraminfo.Length < 2 && args.Count <= 0) return;
            int i = 0;
            while (i + 1 < paraminfo.Length)
            {
                String paramName = paraminfo[i].Name + "Specified";
                if ((paraminfo[i + 1].ParameterType.Equals(typeof(System.Boolean))) && 
                    paramName.Equals(paraminfo[i + 1].Name)) args.Insert(i + 1, true);
                i = i + 2;
            }
        }
        /// <summary>
        /// Add true objects for the Specified fields
        /// </summary>
        /// <param name="args">List of parameters for a methodcall</param>
        /// <param name="m">Methodinfo</param>
        public static int AddTrueForSpecified(List<ParameterInfo> parameterResult, MethodInfo m)
        {
            ParameterInfo[] parameters = m.GetParameters();
            int i = 0;
            int parameterLength = 0;
            while (i + 1 < parameters.Length)
            {
                String paramName = parameters[i].Name + "Specified";
                if ((parameters[i + 1].ParameterType.Equals(typeof(System.Boolean))) && paramName.Equals(parameters[i + 1].Name))
                {
                    parameterResult.Remove(parameters[i + 1]);
                    parameterLength++;
                }
                i = i + 2;
            }
            return parameterLength;
        }
        /// <summary>
        /// Tests if the list of type names are equal to the types of the method parameter.
        /// </summary>
        /// <param name="typeStrings"></param>
        /// <param name="parameterInfos"></param>
        /// <returns></returns>
        public static bool TypeModuleAreEqual(String args, ParameterInfo[] parameterInfos)
        {            
            OpenEngSBModelWrapper element = (OpenEngSBModelWrapper)(new JsonMarshaller()).UnmarshallObject(args, typeof(OpenEngSBModelWrapper));
            if (element == null) throw new ArgumentException("No method could be found");
            return TypesAreEqual(new List<String>(){element.modelClass},parameterInfos);
        }
        /// <summary>
        /// Tests if the list of type names are equal to the types of the method parameter.
        /// </summary>
        /// <param name="typeStrings"></param>
        /// <param name="parameterInfos"></param>
        /// <returns></returns>
        public static bool TypesAreEqual(IList<string> typeStrings, ParameterInfo[] parameterInfos)
        {
            for (int i = 0; i < parameterInfos.Length; ++i)
            {
                if (!TypeIsEqual(typeStrings[i], parameterInfos[i].ParameterType, parameterInfos)) return false;
            }
            return true;
        }

        /// <summary>
        /// Test if two types are equal
        /// </summary>
        /// <param name="remoteType">Remote Type</param>
        /// <param name="localType">Local Type</param>
        /// <returns>If to types are equal</returns>
        private static bool TypeIsEqual(string remoteType, Type localType, ParameterInfo[] parameterInfos)
        {
            if (localType.Equals(typeof(object))) return true;            
            RemoteType remote_typ = new RemoteType(remoteType, parameterInfos);            
            if (localType.Name.ToLower().Contains("nullable")) return (localType.FullName.Contains(remote_typ.Name));            
            return (remote_typ.Name.ToUpper().Equals(localType.Name.ToUpper()));
        }

    }
}
