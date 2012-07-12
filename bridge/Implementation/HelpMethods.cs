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
using System.Reflection;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.RemoteObjects;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation
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
        public static String createClassWithPackageName(String fieldname, Type type)
        {
            String result = null;
            //axis plugin
            MethodInfo method = type.GetMethod(fieldname);
            //Tests if it is a Mehtod or a Type
            if (method != null)
            {
                result = searchSoapAttributes(method);
            }
            else
            {
                result = searchInTheXMLType(fieldname, type);
            }
            return result + "." + HelpMethods.firstLetterToUpper(type.FullName.Replace(type.Namespace + ".", ""));
        }
        /// <summary>
        /// Searches for the packagenames in the XMLType Attribute
        /// </summary>
        /// <param name="fieldname">Typename</param>
        /// <param name="type">IMplementation of the domain (dll)</param>
        /// <returns>Packagename</returns>
        private static String searchInTheXMLType(String fieldname, Type type)
        {
            Assembly ass = type.Assembly;
            type = ass.GetType(fieldname);
            XmlTypeAttribute attribute = (XmlTypeAttribute)Attribute.GetCustomAttributes(type).First(element => element is XmlTypeAttribute);
            if (attribute != null)
            {
                return reverseURL(attribute.Namespace);
            }
            throw new MethodAccessException("Fieldname doesn't have a corresponding attribute (Namepspace) or the attribute couldn't be found");
        }
        /// <summary>
        /// Searches for Packagenames in the SOAP attributes
        /// </summary>
        /// <param name="method">Method to check the SOAP attribute</param>
        /// <returns>Packagename</returns>
        private static String searchSoapAttributes(MethodInfo method)
        {
            SoapDocumentMethodAttribute attribute = (SoapDocumentMethodAttribute)method.GetCustomAttributes(false).First(element => element is SoapDocumentMethodAttribute);
            if (attribute != null)
            {
                return reverseURL(attribute.RequestNamespace);
            }
            throw new MethodAccessException("Fieldname doesn't have a corresponding attribute (Namepspace) or the attribute couldn't be found");
        }
        /// <summary>
        /// Makes the first character to a upper character
        /// </summary>
        /// <param name="element">Element to edit</param>
        /// <returns>String with the first character upper</returns>
        private static String firstLetterToUpper(String element)
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
        public static int addTrueForSpecified(List<ParameterInfo> parameterResult, MethodInfo m)
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