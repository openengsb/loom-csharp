#region Copyright
// <copyright file="ExtendMethods.cs" company="OpenEngSB">
// Licensed to the Austrian Association for Software Tool Integration (AASTI)
// under one or more contributor license agreements. See the NOTICE file
// distributed with this work for additional information regarding copyright
// ownership. The AASTI licenses this file to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation
{
    /// <summary>
    /// This methods uses the .Net 4.0 feature: Extend Methods.
    /// </summary>
    public static class ExtendMethods
    {
        #region Private Static Variables
        private static IMarshaller marshaller = new JsonMarshaller();
        #endregion
        #region Public Static Methods
        /// <summary>
        /// Add a OpenEngSBModel to an Object
        /// </summary>
        /// <typeparam name="ReturnTyp">The Type that gets exptendet with the OpenEngSBModel Interface</typeparam>
        /// <param name="element">The object, which gets extended</param>
        /// <param name="models">The OpenEngSBEntries</param>
        /// <returns>The object with the OpenEngSBModel</returns>
        public static ReturnTyp AddOpenEngSBModel<ReturnTyp>(this ReturnTyp element, List<openEngSBModelEntry> models)
        {
            Type openEngSBModelType = HelpMethods.ImplementTypeDynamicly(element.GetType(), typeof(IOpenEngSBModel));
            IOpenEngSBModel tmpElement = element.ConvertOSBType(openEngSBModelType) as IOpenEngSBModel;
            tmpElement.OpenEngSBModelTail = models;
            return (ReturnTyp)tmpElement;
        }

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
                
                // This can happen if the key is not a primitiv type. It can be that the type is in json and have to be converted
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

        /// <summary>
        /// Converts a Dictionary to a Map (entryX)
        /// </summary>
        /// <typeparam name="ReturnTyp">Type to convert the Dictionary into</typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static ReturnTyp[] ConvertMap<ReturnTyp>(this IDictionary arg)
        {
            Array elements = ConvertMap(arg, typeof(ReturnTyp));
            return (ReturnTyp[])elements;
        }

        /// <summary>
        /// Converts a Map (WSDL converted Type i.e entryX) to an Dictionary.
        /// If the Object is not a Map then the parameter object is returned
        /// </summary>
        /// <typeparam name="KeyTyp">Key type</typeparam>
        /// <typeparam name="ValueTyp">Value Type</typeparam>
        /// <param name="obj">Map input</param>
        /// <returns>IDictionary</returns>
        public static IDictionary<KeyTyp, ValueTyp> ConvertMap<KeyTyp, ValueTyp>(this Object obj)
        {
            Object result = ConvertMap(obj);
            String test = result.GetType().Name;
            IDictionary tmpDict = (IDictionary)result;
            IDictionary<KeyTyp, ValueTyp> tmpresult = new Dictionary<KeyTyp, ValueTyp>();
            foreach (Object key in tmpDict.Keys)
            {
                tmpresult.Add((KeyTyp)key, (ValueTyp)tmpDict[key]);
            }

            return tmpresult;
        }

        /// <summary>
        /// Converts a Map (WSDL converted Type i.e entryX) to an Dictionary.
        /// If the Object is not a Map then the parameter object is returned
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <returns>IDictionary or the object itselfe</returns>
        public static Object ConvertMap(this Object obj)
        {
            Object array = obj;
            if (!obj.GetType().IsArray)
            {
                if (!obj.GetType().Name.ToUpper().Contains("ENTRY"))
                {
                    throw new ArgumentOutOfRangeException("The object with type: " + obj.GetType().Name + " could ist not valid. It has to start with ENTRY");
                }
                else
                {
                    array = new Object[] { obj };
                }
            }

            Dictionary<Object, Object> result = new Dictionary<Object, Object>();
            foreach (object keyValue in (Object[])array)
            {
                Object key = keyValue.GetType().GetProperty("key").GetValue(keyValue, null);
                Object value = keyValue.GetType().GetProperty("value").GetValue(keyValue, null);
                result.Add(key, value);
            }

            return result;
        }

        /// <summary>
        /// Converts between two types
        /// </summary>
        /// <typeparam name="ReturnType">Resulting type</typeparam>
        /// <param name="obj">object that will be converted</param>
        /// <returns>Returns the new Object with Type T</returns>
        public static ReturnType ConvertOSBType<ReturnType>(this Object obj)
        {
            String tmp = marshaller.MarshallObject(obj);
            return marshaller.UnmarshallObject<ReturnType>(tmp);
        }

        /// <summary>
        /// Converts between two types
        /// </summary>
        /// <param name="obj">object that will be converted</param>
        /// <param name="type">Resulting type</param>
        /// <returns>Returns the new Object with Type T</returns>
        public static object ConvertOSBType(this Object obj, Type type)
        {
            String tmp = marshaller.MarshallObject(obj);
            return marshaller.UnmarshallObject(tmp, type);
        }
        #endregion
        #region Private Static Methods
        private static bool FalseIfBothValuesAreFalse(Boolean typeNamesAreTheSame, Boolean typeAreTheSame)
        {
            return !(typeNamesAreTheSame || typeAreTheSame);
        }

        /// <summary>
        /// Checks if to type have the same type
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyType"></param>
        /// <returns></returns>
        private static bool IsValueNotInCorrectType(Object key, Type keyType)
        {
            Boolean typeNamesAreTheSame = keyType.Name.Equals(key.GetType().Name);
            Boolean typeAreTheSame = keyType.IsInstanceOfType(key.GetType().DeclaringType);
            return FalseIfBothValuesAreFalse(typeNamesAreTheSame, typeAreTheSame);
        }
        #endregion
    }
}