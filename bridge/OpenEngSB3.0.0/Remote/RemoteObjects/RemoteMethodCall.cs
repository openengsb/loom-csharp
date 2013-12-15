#region Copyright
// <copyright file="RemoteMethodCall.cs" company="OpenEngSB">
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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.RemoteObjects;

namespace Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects
{
    /// <summary>
    /// This class represents a RPC with its parameters, return types etc.
    /// </summary>
    public class RemoteMethodCall : IMethodCall
    {
        #region Variables

        /// <summary>
        /// Fully qualified class names of the arguments.
        /// </summary>
        public IList<string> Classes { get; set; }

        /// <summary>
        /// Name of the method to be called.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Arguments of the call.
        /// </summary>
        public IList<object> Args { get; set; }

        /// <summary>
        /// Metadata
        /// </summary>
        [JsonProperty(PropertyName = "metaData")]
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// Include the packagestruktur on the java side
        /// </summary>
        [JsonProperty(PropertyName = "realClassImplementation")]
        public IList<string> RealClassImplementation { get; set; }

        #endregion

        #region Public static Methods

        public static RemoteMethodCall CreateInstance(string methodName, IList<object> args, IDictionary<string, string> metaData, IList<string> classes, IList<String> realClassImplementation)
        {
            RemoteMethodCall call = new RemoteMethodCall();
            call.MethodName = methodName;
            call.Args = args;
            call.MetaData = metaData;
            call.Classes = classes;
            call.RealClassImplementation = realClassImplementation;
            return call;
        }

        #endregion
    }
}