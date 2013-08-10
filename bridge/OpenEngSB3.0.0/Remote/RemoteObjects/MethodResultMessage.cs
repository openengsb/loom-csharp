#region Copyright
// <copyright file="MethodResultMessage.cs" company="OpenEngSB">
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
using Newtonsoft.Json;

namespace Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects
{
    /// <summary>
    /// Container for MethodResultMessage
    /// </summary>
    public class MethodResultMessage : MessageBase
    {
        #region Variables
        [JsonProperty(PropertyName = "result")]
        public MethodResult Result
        {
            get;
            set;
        }

        #endregion

        #region Public static Methods

        /// <summary>
        /// Creates a new instance of MethodResultMessage
        /// </summary>
        /// <param name="result">Message</param>
        /// <returns>Returns a new instance of MethodResultMessage</returns>
        public static MethodResultMessage CreateInstance(MethodResult result, String callid)
        {
            MethodResultMessage instance = new MethodResultMessage();
            instance.Result = result;
            instance.CallId = callid;
            instance.Timestamp = DateTime.Now.Ticks;
            return instance;
        }

        #endregion
    }
}