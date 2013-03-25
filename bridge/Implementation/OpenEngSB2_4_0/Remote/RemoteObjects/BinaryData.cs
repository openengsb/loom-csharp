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
using System.Diagnostics.CodeAnalysis;
namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB2_4_0.Remote.RemoteObjects
{
    //Will be removed because the .Net Bridge starts supporing the OpenEngSB Version 3.0.0
    [ExcludeFromCodeCoverageAttribute()]
    /// <summary>
    /// Container for the Binary Data
    /// </summary>
    public class BinaryData
    {
        #region Public Static Methods
        /// <summary>
        /// Creates an Instance of BinaryData
        /// </summary>
        /// <returns>A new instance of BinarayData</returns>
        public static BinaryData CreateInstance()
        {
            return new BinaryData();
        }
        #endregion
    }
}