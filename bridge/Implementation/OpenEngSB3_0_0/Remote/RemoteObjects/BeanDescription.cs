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

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects
{
    /// <summary>
    /// Container for the Authentification
    /// </summary>
    public class BeanDescription
    {
        #region Variables

        public String className { get; set; }
        public IDictionary<String, String> data { get; set; }
        public IDictionary<String, byte[]> binaryData { get; set; }

        #endregion
        #region Public static Methods

        /// <summary>
        /// Creates a new instance of the Authentification
        /// </summary>
        /// <param name="className">ClassName</param>
        /// <returns>A new instance of Authentification</returns>
        public static BeanDescription createInstance(String className)
        {
            BeanDescription instance = new BeanDescription();
            instance.className = className;
            instance.data = new Dictionary<String, String>();
            instance.binaryData = new Dictionary<String, byte[]>();
            return instance;
        }

        #endregion
    }
}
