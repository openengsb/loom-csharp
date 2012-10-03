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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using log4net;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation
{
    public partial class RegistrationFunctions
    {
        private ILog logger;
        
        public RegistrationFunctions(ILog logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void setDomainId(String element)
        {
            logger.Info("setDomainId:" + element);
        }
        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void setConnectorId(String element)
        {
            logger.Info("setConnectorId:" + element);
        }
        /// <summary>
        /// Get invokes, when the OpenEngSB checks the connections
        /// </summary>
        /// <returns></returns>
        public AliveState getAliveState()
        {
            logger.Info("GetAliveState called. Answering with ONLINE");
            return AliveState.ONLINE;
        }

    
    }
}

