#region Copyright
// <copyright file="MethodResult.cs" company="OpenEngSB">
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
using System.Collections.Generic;
using Newtonsoft.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.RemoteObjects;

namespace Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects
{
    /// <summary>
    /// This class represents the return value of an RPC.
    /// </summary>
    public class MethodResult : IMethodResult
    {
        #region Variables

        /// <summary>
        /// Type of the return value.
        /// </summary>
        public ReturnType Type { get; set; }

        /// <summary>
        /// Return value of the RPC.
        /// </summary>
        public object Arg { get; set; }

        /// <summary>
        /// Metadata
        /// </summary>
        [JsonProperty(PropertyName = "metaData")]
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// Fully qualified class name of the return value.
        /// </summary>
        [JsonProperty(PropertyName = "className")]
        public string ClassName { get; set; }

        #endregion

        #region Public static Methods

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="type">Returntype</param>
        /// <param name="arg">Arguments</param>
        /// <param name="metaData">MetaDatas</param>
        /// <param name="className">ClassName</param>
        /// <returns>New instance of MethodResult</returns>
        public static MethodResult CreateInstance(ReturnType type, object arg, IDictionary<string, string> metaData, string className)
        {
            MethodResult result = new MethodResult();
            result.Type = type;
            result.Arg = arg;
            result.MetaData = metaData;
            result.ClassName = className;
            return result;
        }

        #endregion
    }
}
