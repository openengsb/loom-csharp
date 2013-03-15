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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation
{
    public static class ExtendMethods
    {
        public static IMarshaller marshaller = new JsonMarshaller();
        /// <summary>
        /// Converts a Dictionary to a Map (entryX)
        /// </summary>
        /// <param name="arg">IDcitionary</param>
        /// <param name="type">type to convert it into</param>
        /// <returns></returns>
        public static Object[] ConvertMap(this IDictionary arg, Type type)
        {
            Type elementType = type;
            if (elementType.IsArray)
            {
                elementType = type.GetElementType();
            }
            Array elements = Array.CreateInstance(elementType, arg.Count);

            int i = 0;
            foreach (Object key in arg.Keys)
            {
                Object instance = Activator.CreateInstance(elementType, false);
                Object value = arg[key];
                Object keyobject = key;
                Type keyType = elementType.GetProperty("key").PropertyType;
                Type valueType = elementType.GetProperty("value").PropertyType;

                if (IsValueNotInCorrectType(key, keyType))
                {
                    keyobject = marshaller.UnmarshallObject(key.ToString(), keyType);
                }
                elementType.GetProperty("key").SetValue(instance, keyobject, null);
                if (IsValueNotInCorrectType(value, valueType))
                {
                    value = marshaller.UnmarshallObject(value.ToString(), valueType);
                }
                elementType.GetProperty("value").SetValue(instance, value, null);
                elements.SetValue(instance, i++);
            }
            return (Object[])elements;
        }
        public static T ConvertOSBType<T>(this Object obj)
        {
            String tmp = marshaller.MarshallObject(obj);
            return marshaller.UnmarshallObject<T>(tmp);
        }
        /// <summary>
        /// Checks if to type have the same type
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyType"></param>
        /// <returns></returns>
        private static bool IsValueNotInCorrectType(Object key, Type keyType)
        {
            Boolean a1 = keyType.Name.Equals(key.GetType().Name);
            Boolean a2 = keyType.IsInstanceOfType(key.GetType().DeclaringType);
            return !(a1 || a2);
        }
        /// <summary>
        /// Converts a Dictionary to a Map (entryX)
        /// </summary>
        /// <typeparam name="T">Type to convert the Dictionary into</typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static T[] ConvertMap<T>(this IDictionary arg)
        {
            Array elements = ConvertMap(arg, typeof(T));
            return (T[])elements;
        }
        /// <summary>
        /// Converts a Map (WSDL converted Type i.e entryX) to an Dictionary.
        /// If the Object is not a Map then the parameter object is returned
        /// </summary>
        /// <typeparam name="T">Key type</typeparam>
        /// <typeparam name="V">Value Type</typeparam>
        /// <param name="obj">Map input</param>
        /// <returns>IDictionary</returns>
        public static IDictionary<T, V> ConvertMap<T, V>(this Object obj)
        {
            Object result = ConvertMap(obj);
            String test = result.GetType().Name;
            try
            {
                IDictionary tmpDict = ((IDictionary)result);

                IDictionary<T, V> tmpresult = new Dictionary<T, V>();
                foreach (Object key in tmpDict.Keys)
                {
                    tmpresult.Add((T)key, (V)tmpDict[key]);
                }
                return tmpresult;
            }
            catch
            {
                throw new BridgeException("Unable to Convert the Object to a Dictionary");
            }
        }
        /// <summary>
        /// Converts a Map (WSDL converted Type i.e entryX) to an Dictionary.
        /// If the Object is not a Map then the parameter object is returned
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <returns>IDictionary or the object itselfe</returns>
        public static Object ConvertMap(this Object obj)
        {            
            if (!(obj.GetType().IsArray) || !obj.GetType().Name.ToUpper().Contains("ENTRY"))
            {
                return obj;
            }

            Dictionary<Object, Object> result = new Dictionary<Object, Object>();
            foreach (object keyValue in (Object[])obj)
            {
                Object key = keyValue.GetType().GetProperty("key").GetValue(keyValue, null);
                Object value = keyValue.GetType().GetProperty("value").GetValue(keyValue, null);
                result.Add(key, value);
            }

            return result;
        }
   
    }
}