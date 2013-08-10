#region Copyright
// <copyright file="IMethodCall.cs" company="OpenEngSB">
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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.RemoteObjects
{
    /// <summary>
    /// The names have to be in java coding style, else the OpenEngSB marshaller can not find the correct classes
    /// </summary>
    public interface IMethodCall
    {
        #region Properties
        /// <summary>
        /// Arguments of the call.
        /// </summary>
        [JsonProperty(PropertyName = "args")]
        IList<object> Args
        {
            get;
            set;
        }

        /// <summary>
        /// Fully qualified class names of the arguments.
        /// </summary>
        [JsonProperty(PropertyName = "classes")]
        IList<string> Classes
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the method to be called.
        /// </summary>
        [JsonProperty(PropertyName = "methodName")]
        String MethodName
        {
            get;
            set;
        }
        #endregion
    }
}