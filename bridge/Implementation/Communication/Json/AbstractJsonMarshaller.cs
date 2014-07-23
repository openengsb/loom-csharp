#region Copyright
// <copyright file="AbstractJsonMarshaller.cs" company="OpenEngSB">
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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json
{
    public abstract class AbstractJsonMarshaller : JsonConverter
    {
        #region Public Methods
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            serializer.NullValueHandling = NullValueHandling.Ignore;
            if (IsMapType(objectType))
            {
                IDictionary list = new Dictionary<Object, Object>();
                serializer.Populate(reader, list);
                return list.ConvertMap(objectType);
            }
            else if (IsException(objectType))
            {
                Object exceptionObject = Activator.CreateInstance(HelpMethods.ImplementTypeDynamicly(objectType, typeof(IJavaException)));
                serializer.Populate(reader, exceptionObject);
                return exceptionObject;
            }

            Object modelWithOpenEngsbModelTail = Activator.CreateInstance(HelpMethods.ImplementTypeDynamicly(objectType, typeof(IOpenEngSBModel)));
            serializer.Populate(reader, modelWithOpenEngsbModelTail);
            return modelWithOpenEngsbModelTail;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (IsMapType(value.GetType()))
            {
                Object tmp = value.ConvertMap();
                serializer.Serialize(writer, tmp);
            }
            else
            {
                CreateJsonWithoutXMLIgnoreFields(writer, value, serializer);
            }
        }
        #endregion
        #region Protected Methods
        /// <summary>
        /// Checks if Type is Map (Dictaionary
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected bool IsMapType(Type objectType)
        {
            return objectType.Name.ToUpper().Contains("MAPENTRY") && objectType.IsArray;
        }

        /// <summary>
        /// This method checks, if a Type can be extended with IOpenEngSBModel.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected Boolean CanTypeBeExtendedWithOpenEngsbModelTail(Type objectType)
        {
            return !(isObjectType(objectType) || isBasicType(objectType) || isOpenEngSBModelTypeImplemented(objectType) || objectType.IsArray || isCollection(objectType) || objectType.IsEnum);
        }

        private Boolean isObjectType(Type objectType)
        {
            return objectType.Name.ToUpper().Equals("OBJECT"); 
        }

        private Boolean isBasicType(Type objectType)
        {
            return objectType.IsPrimitive || objectType.Name.ToUpper().EndsWith("STRING");
        }

        private Boolean isCollection(Type objectType)
        {
            return objectType.GetInterface(typeof(ICollection).Name) != null || objectType.GetInterface(typeof(ICollection<>).Name) != null;
        }

        private Boolean isOpenEngSBModelTypeImplemented(Type objectType)
        {
            return objectType.GetInterfaces().Contains(typeof(IOpenEngSBModel));
        }
        /// <summary>
        /// Checks if the type is an Exception type (Ends with Exception)
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected bool IsException(Type objectType)
        {
            return objectType.Name.ToUpper().Contains("EXCEPTION");
        }
        #endregion
        #region Private Methods
        private void CreateJsonWithoutXMLIgnoreFields(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            foreach (PropertyInfo pi in value.GetType().GetProperties())
            {
                if (!pi.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    writer.WritePropertyName(pi.Name);
                    serializer.Serialize(writer, pi.GetValue(value, null));
                }
            }

            writer.WriteEndObject();
        }

        private Boolean TestIfNullValueProducesTheException(JsonSerializationException jsonex)
        {
            return jsonex.Message.ToUpper().Contains("NULL");
        }
        #endregion
    }
}