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

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.OpenEngSB2_4_0.Remote
{
    /// <summary>
    /// This class represents the return value of an RPC.
    /// </summary>
    public class MethodResult
    {
        #region Variables
        public enum ReturnType { Void, Object, Exception }

        /// <summary>
        /// Type of the return value.
        /// </summary>
		public ReturnType type { get; set; }

        /// <summary>
        /// Return value of the RPC.
        /// </summary>
		public object arg { get; set; }

        /// <summary>
        /// Metadata
        /// </summary>
		public IDictionary<string, string> metaData { get; set; }

        /// <summary>
        /// Fully qualified class name of the return value.
        /// </summary>
		public string className { get; set; }
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
            result.type = type;
            result.arg = arg;
            result.metaData = metaData;
            result.className = className;
            return result;
        }
        #endregion
    }
}
