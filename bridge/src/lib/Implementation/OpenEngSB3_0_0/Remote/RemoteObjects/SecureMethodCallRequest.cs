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

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects
{
    /// <summary>
    /// Container for a Secured Method Call
    /// </summary>
    public class SecureMethodCallRequest
    {
        #region Variables
        public String principal { get; set; }
        public AuthenticationInfo credentials { get; set; }
        public long timestamp { get; set; }
        public Message message { get; set; }
        #endregion
        #region Public Static Methods
        /// <summary>
        /// Creates an instance of SecureMethodCallRequest
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <param name="credentials">Credentials</param>
        /// <param name="message">Message</param>
        /// <returns>Instance of SecureMethodCallRequest</returns>
        public static SecureMethodCallRequest createInstance(String principal, AuthenticationInfo credentials, Message message)
        {
            SecureMethodCallRequest instance = new SecureMethodCallRequest();
            instance.principal = principal;
            instance.credentials = credentials;
            instance.timestamp = DateTime.Now.Ticks;
            instance.message = message;
            return instance;
        }
        #endregion
    }
}
