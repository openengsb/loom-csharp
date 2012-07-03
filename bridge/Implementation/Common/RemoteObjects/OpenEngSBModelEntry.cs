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

namespace Implementation.Common.RemoteObjects
{
    public class OpenEngSBModelEntry
    {
        public String key { get; set; }
        public String value { get; set; }
        public String type { get; set; }
        public OpenEngSBModelEntry() { }
        public static OpenEngSBModelEntry getInstance(String key, String value, String type)
        {
            OpenEngSBModelEntry result = new OpenEngSBModelEntry();
            result.key = key;
            result.value = value;
            result.type = type;
            return result;
        }
    }
}
