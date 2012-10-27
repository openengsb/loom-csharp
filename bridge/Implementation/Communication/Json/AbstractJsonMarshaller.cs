﻿/***
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json
{
    public abstract class AbstractJsonMarshaller : JsonConverter{
       
        protected static bool isMapType(Type objectType)
        {
            return objectType.Name.ToUpper().Contains("ENTRY");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (isMapType(objectType))
            {

                IDictionary list = new Dictionary<Object, Object>();
                serializer.Populate(reader, list);
                return list.ConvertMap(objectType);
            }

            serializer.Populate(reader, existingValue);
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (isMapType(value.GetType()))
            {
                Object tmp = value.ConvertMap();
                serializer.Serialize(writer, tmp);
            }
            else
            {
                CreateJsonWithoutXMLIgnoreFields(writer, value, serializer);
            }
        }

        private static void CreateJsonWithoutXMLIgnoreFields(JsonWriter writer, object value, JsonSerializer serializer)
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
    }
}

