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
using System.IO;
using Newtonsoft.Json;

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Communication.Json
{
    /// <summary>
    /// This class converts any given object to an json-message.
    /// </summary>
    public class JsonMarshaller : IMarshaller
    {
        #region Methods
        /// <summary>
        /// Uses the Newtonsoft Json Parser, to deserialize the jsontext. The fastJson deserializer has problems to deserialize the objects
        /// </summary>
        /// <param name="jsonText">Json Object</param>
        /// <param name="objectType">Object Typ</param>
        /// <returns>Deserialize Objects</returns>
        public object UnmarshallObject(string jsonText, Type objectType)
        {
            return JsonConvert.DeserializeObject(jsonText, objectType);
        }

        /// <summary>
        /// Use fastjson to create the Json Message. fastjson doesn't serialize [XMLIgnore] field
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>Returns a Json Message</returns>
        public string MarshallObject(object obj)
        {
            fastJSON.JSON json=fastJSON.JSON.Instance;
            json.IndentOutput=false;
            json.SerializeNullValues=true;
            json.ShowReadOnlyProperties=false;
            json.UseFastGuid=false;
            json.UseOptimizedDatasetSchema=false;
            json.UseSerializerExtension=false;
            json.UseUTCDateTime=false;
            json.UsingGlobalTypes=false;            
            return json.ToJSON(obj);
        }
        #endregion
    }
}
