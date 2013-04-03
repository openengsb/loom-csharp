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
using System.Collections;
using System.Reflection.Emit;
using System.Threading;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation
{
    public static class HelpMethods
    {

        /// <summary>
        /// Implemnents the OpenEngSBModel type to a sepcified type
        /// </summary>
        /// <param name="extendType">Type to extend</param>
        /// <returns>Type:OpenEngSBModel</returns>
        public static Type ImplementTypeDynamicly(Type extendType)
        {
            if (extendType.Name.ToUpper().Equals("OBJECT") || extendType.IsPrimitive || extendType.Name.ToUpper().Equals("STRING"))
            {
                return extendType;
            }
            AssemblyName assemblyName = new AssemblyName("DataBuilderAssembly");
            AssemblyBuilder assemBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemBuilder.DefineDynamicModule("DataBuilderModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("OpenEngSBModel" + extendType.Name, TypeAttributes.Class, extendType);
            typeBuilder.AddInterfaceImplementation(typeof(OpenEngSBModel));
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            BuildProperty(typeBuilder);
            Type type = typeBuilder.CreateType();
            return type;
        }

        /// <summary>
        /// Generate the set and get
        /// </summary>
        /// <param name="typeBuilder"></param>
        private static void BuildProperty(TypeBuilder typeBuilder)
        {
            foreach (PropertyInfo method in typeof(OpenEngSBModel).GetProperties())
            {
                Type type = method.GetGetMethod().ReturnType;
                if (!type.Name.Equals(typeof(void)))
                {
                    String name = method.Name;
                    FieldBuilder field = typeBuilder.DefineField(name, type, FieldAttributes.Private);
                    PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, type, null);
                    MethodAttributes getSetAttr = MethodAttributes.Public |
                            MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual;
                    MethodBuilder getter = typeBuilder.DefineMethod("get_" + name, getSetAttr, type, Type.EmptyTypes);
                    ILGenerator getIL = getter.GetILGenerator();
                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, field);
                    getIL.Emit(OpCodes.Ret);
                    MethodBuilder setter = typeBuilder.DefineMethod("set_" + name, getSetAttr, null, new Type[] { type });
                    ILGenerator setIL = setter.GetILGenerator();
                    setIL.Emit(OpCodes.Ldarg_0);
                    setIL.Emit(OpCodes.Ldarg_1);
                    setIL.Emit(OpCodes.Stfld, field);
                    setIL.Emit(OpCodes.Ret);
                    propertyBuilder.SetGetMethod(getter);
                    propertyBuilder.SetSetMethod(setter);
                }
            }
        }

        /// <summary>
        /// Takes a Namespace as input, reverse the elements and returns the package structure from java
        /// </summary>
        /// <param name="url">Namespace URL</param>
        /// <returns>Java Package structure</returns>
        public static String ReverseURL(String url)
        {
            String tmp = url.Replace("http://", "");
            if (tmp.Contains("/")) tmp = tmp.Substring(0, tmp.IndexOf("/"));
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
        public static String CreateClassWithPackageName(String fieldname, Type type)
        {
            String result = null;
            //axis plugin
            MethodInfo method = type.GetMethod(fieldname);
            //Tests if it is a Mehtod or a Type
            if (method != null)
            {
                result = SearchSoapAttributes(method);
            }
            else
            {
                result = SearchInXMLType(fieldname, type);
            }
            String classname = HelpMethods.FirstLetterToUpper(type.FullName.Replace(type.Namespace + ".", ""));
            if (classname.Contains("[]"))
            {
                return "[L" + result + "." + classname.Replace("[]", "") + ";";
            }
            else
            {
                return result + "." + classname;
            }
        }

        /// <summary>
        /// Searches for the packagenames in the XMLType Attribute
        /// </summary>
        /// <param name="fieldname">Typename</param>
        /// <param name="type">IMplementation of the domain (dll)</param>
        /// <returns>Packagename</returns>
        private static String SearchInXMLType(String fieldname, Type type)
        {
            String exceptionMessage = "Fieldname doesn't have a corresponding attribute (Namepspace) or the attribute couldn't be found";
            String typename = fieldname;
            if (typename.Contains("[]"))
            {
                typename = fieldname.Substring(0, fieldname.IndexOf("[]"));
            }
            Assembly ass = type.Assembly;

            type = ass.GetType(typename);
            if (type == null)
            {
                throw new BridgeException(exceptionMessage);
            }
            XmlTypeAttribute attribute = (XmlTypeAttribute)Attribute.GetCustomAttributes(type).FirstOrDefault(element => element is XmlTypeAttribute);
            if (attribute != null)
            {
                return ReverseURL(attribute.Namespace);
            }
            throw new BridgeException(exceptionMessage);
        }

        /// <summary>
        /// Searches for Packagenames in the SOAP attributes
        /// </summary>
        /// <param name="method">Method to check the SOAP attribute</param>
        /// <returns>Packagename</returns>
        private static String SearchSoapAttributes(MethodInfo method)
        {
            SoapDocumentMethodAttribute attribute = (SoapDocumentMethodAttribute)method.GetCustomAttributes(false).FirstOrDefault(element => element is SoapDocumentMethodAttribute);
            if (attribute != null)
            {
                return ReverseURL(attribute.RequestNamespace);
            }
            throw new BridgeException("Fieldname doesn't have a corresponding attribute (Namepspace) or the attribute couldn't be found");
        }

        /// <summary>
        /// Makes the first character to a upper character
        /// </summary>
        /// <param name="element">Element to edit</param>
        /// <returns>String with the first character upper</returns>
        public static String FirstLetterToUpper(String element)
        {
            if (element.Length <= 1)
            {
                return element.ToUpper();
            }
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
        public static void AddTrueForSpecified(IList<object> args, MethodInfo m)
        {
            ParameterInfo[] paraminfo = m.GetParameters();
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
            int parameterLength = parameterResult.Count;
            while (i + 1 < parameters.Length)
            {
                String paramName = parameters[i].Name + "Specified";
                Boolean isBoolean = parameters[i + 1].ParameterType.Equals(typeof(System.Boolean));
                Boolean isParameterTheSame = paramName.Equals(parameters[i + 1].Name);
                if (isBoolean && isParameterTheSame)
                {
                    parameterResult.Remove(parameters[i + 1]);
                    parameterLength--;
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
            if (typeStrings.Count != parameterInfos.Length)
            {
                throw new BridgeException("The method has not the same amount of parameters");
            }
            for (int i = 0; i < parameterInfos.Length; ++i)
            {
                if (!TypeIsEqual(typeStrings[i], parameterInfos[i].ParameterType, parameterInfos))
                {
                    return false;
                }
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
            if (localType.Equals(typeof(object)))
            {
                return true;
            }
            RemoteType remote_typ = new RemoteType(remoteType, parameterInfos);
            if (localType.Name.ToLower().Contains("nullable"))
            {
                return (localType.FullName.Contains(remote_typ.Name));
            }
            return (remote_typ.Name.ToUpper().Equals(localType.Name.ToUpper()));
        }
    }
}