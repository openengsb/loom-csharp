﻿/***
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

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.OpenEngSB2_4_0.Remote
{
    /// <summary>
    /// Container for MessageResult
    /// </summary>
    public class MessageResult
    {
        #region Variables
        public MethodResult result { get; set; }
        public string callId { get; set; }
        #endregion
        #region Public static Method
        /// <summary>
        /// Creates a new instance of MessageResult
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="callId">CallId</param>
        /// <returns>returns a new instance of MessageResult</returns>
        public static MessageResult CreateInstance(MethodResult result, string callId)
        {
            MessageResult msg = new MessageResult();
            msg.result = result;
            msg.callId = callId;
            return msg;
        }
        #endregion
    }
}
