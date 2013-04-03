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

namespace Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects
{
    /// <summary>
    /// Container for a Secured Method Call
    /// </summary>
    public class MethodCallMessage : MessageBase
    {
        #region Variables

        public RemoteMethodCall methodCall { get; set; }

        public bool answer { get; set; }

        public string destination { get; set; }

        public String principal { get; set; }

        public BeanDescription credentials { get; set; }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates an instance of SecureMethodCallRequest
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <param name="credentials">Credentials</param>
        /// <param name="message">Message</param>
        /// <returns>Instance of SecureMethodCallRequest</returns>
        public static MethodCallMessage createInstance(String principal, BeanDescription credentials, RemoteMethodCall methodCall, string callId, bool answer, string destination)
        {
            MethodCallMessage instance = new MethodCallMessage();
            instance.methodCall = methodCall;
            instance.callId = callId;
            instance.answer = answer;
            instance.destination = destination;
            instance.principal = principal;
            instance.credentials = credentials;
            instance.timestamp = DateTime.Now.Ticks;
            instance.credentials = credentials;
            return instance;
        }

        #endregion
    }
}