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
using System.Reflection;

namespace Implementation.Common
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
        public RemoteType(string typeString, ParameterInfo[] parameterInfos)
        {
            FullName = typeString;
            if (FullName.Contains("$"))
            {
                Name = FullName.Split('$').Last().Trim();
                LocalTypeFullName = FullName;
                foreach (ParameterInfo par in parameterInfos)
                {
                    if (par.ParameterType.FullName.ToUpper().Contains(Name.ToUpper()))
                    {      
                        LocalTypeFullName = par.ParameterType.FullName;
                    }
                }
            }
            else
            {
                Name = FullName.Split('.').Last().Trim();
                foreach (ParameterInfo par in parameterInfos)
                {
                    if (par.ParameterType.FullName.Contains(Name)|| par.ParameterType.Equals(typeof(object)))
                    {
                        LocalTypeFullName = par.ParameterType.FullName;
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
