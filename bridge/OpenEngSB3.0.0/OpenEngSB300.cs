#region Copyright
// <copyright file="OpenEngSB300.cs" company="OpenEngSB">
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
using System.Linq;
using System.Reflection;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;

namespace Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300
{
    public class OpenEngSB300 : OpenEngSBImplementationSupportManager
    {
        #region Constructor
        /// <summary>
        /// Adds this Assembly to the supportedLists
        /// </summary>
        public OpenEngSB300()
            : base(Assembly.GetExecutingAssembly())
        {
        }
        #endregion
        #region Public Methods
        public static void SetSupport()
        {
            new OpenEngSB300();
        }
        #endregion
    }
}