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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    /// <summary>
    /// This class maps remote types on the bus to local types.
    /// </summary>
    public class LocalType
    {
        #region Variables
        private Type type;
        #endregion
        #region Propreties
        public string RemoteTypeFullName
        {
            get
            {
                if (type.Equals(typeof(string)))
                {
                    return "java.lang.String";
                }
                if (type.Equals(typeof(AliveState)))
                {
                    return "org.openengsb.core.api.AliveState";
                }
                if (type.IsPrimitive){
                    return type.FullName;
                }
                return HelpMethods.createClassWithPackageName(type.FullName, type);
            }
        }
        #endregion
        #region Constructor
        public LocalType(Type type)
        {
            this.type = type;
        }
        #endregion
    }
}
