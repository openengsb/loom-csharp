#region Copyright
// <copyright file="RegistrationFunctions.cs" company="OpenEngSB">
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
using log4net;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation
{
    public partial class RegistrationFunctions
    {
        #region Properties
        public ILog Logger { get; set; }
        #endregion
        #region Constructor
        public RegistrationFunctions()
        {
            this.Logger = LogManager.GetLogger(typeof(RegistrationFunctions));
        }

        public RegistrationFunctions(ILog logger)
        {
            this.Logger = logger;
        }
        #endregion
        #region public Methods
        /// <summary>
        /// Get invokes, when the OpenEngSB checks the connections
        /// </summary>
        /// <returns></returns>
        public AliveState GetAliveState()
        {
            Logger.Info("GetAliveState called. Answering with ONLINE");
            return AliveState.ONLINE;
        }

        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void SetConnectorId(String element)
        {
            Logger.Info("setConnectorId:" + element);
        }

        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void SetDomainId(String element)
        {
            Logger.Info("setDomainId:" + element);
        }
        #endregion
    }
}