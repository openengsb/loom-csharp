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

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Common
{
    /// <summary>
    /// This class maps a local type to a bus side type.
    /// </summary>
    public class RemoteType
    {
        #region Propreties
        public string FullName { get; set; }

        public string Name { get; set; }

        public string LocalTypeFullName { get; set; }
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="typeString">Type in a String format</param>
        public RemoteType(string typeString)
        {
            FullName = typeString;
            Name = FullName.Split('.').Last().Trim();
            SetLocalTypeFullName();
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Find the name of the local Type
        /// </summary>
        public void SetLocalTypeFullName()
        {
            // Workaround: built-in types.
            if (Name == "String")
            {
                LocalTypeFullName = "System.String";
                return;
            }

            StringBuilder builder = new StringBuilder();
            int counter = 1;
            for (int i = 0; i < FullName.Length; ++i)
            {
                if (FullName[i] == '$')
                {
                    builder.Append(counter.ToString());
                }
                else
                {
                    builder.Append(FullName[i]);
                }
            }

            // Workaround: event-keyword
            LocalTypeFullName = builder.ToString().Replace(".event","._event");
        }
        #endregion
    }
}
