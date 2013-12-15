#region Copyright
// <copyright file="RemoteType.cs" company="OpenEngSB">
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
using System.Linq;
using System.Reflection;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    /// <summary>
    /// This class maps a local type to a bus side type.
    /// </summary>
    public class RemoteType
    {
        #region Constructors
        public RemoteType(string typeString, ParameterInfo[] parameterInfos)
        {
            RemoteTypeFullName = typeString;
            if (RemoteTypeFullName.Contains("$"))
            {
                Name = RemoteTypeFullName.Split('$').Last().Trim();
                CreateFullName(Name, parameterInfos);
            }
            else
            {
                CreateFullName(RemoteTypeFullName.Split('.').Last().Trim().Replace(";", String.Empty), parameterInfos);
                Name = LocalTypeFullName.Split('.').Last().Trim();
            }
        }
        #endregion
        #region Properties
        public String LocalTypeFullName
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }

        public String RemoteTypeFullName
        {
            get;
            set;
        }
        #endregion
        #region Private Methods
        private String CheckPrimitivType(String mehtodName)
        {
            if (mehtodName.ToUpper().Contains("INT"))
            {
                return typeof(int).ToString();
            }
            
            if (mehtodName.ToUpper().Contains("STRING"))
            {
                return typeof(string).ToString();
            }
            
            if (mehtodName.ToUpper().Contains("FLOAT"))
            {
                return typeof(float).ToString();
            }
            
            if (mehtodName.ToUpper().Contains("DOUBLE"))
            {
                return typeof(double).ToString();
            }
         
            return null;
        }

        private void CreateFullName(String methodName, ParameterInfo[] parameterInfos)
        {
            if (parameterInfos != null)
            {
                foreach (ParameterInfo par in parameterInfos)
                {
                    if (par.ParameterType.FullName.ToUpper().Contains(methodName.ToUpper()))
                    {
                        LocalTypeFullName = par.ParameterType.FullName;
                        break;
                    }
                }
            }

            if (String.IsNullOrEmpty(LocalTypeFullName))
            {
                LocalTypeFullName = CheckPrimitivType(methodName.Split('.').Last().Trim());
            }

            if (String.IsNullOrEmpty(LocalTypeFullName))
            {
                throw new BridgeException("The type from the OpenEngSB could not be converted");
            }
        }
        #endregion
    }
}