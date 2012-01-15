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
using System.Web.Services;
using System.Text;
using log4net;
namespace Org.OpenEngSB.Loom.Csharp.Common.ServiceTestConsole
{
    /// <summary>
    /// Example implementation of the local domain
    /// </summary>
    class SignalConnector : ISignalDomainSoapBinding
    {
        private ILog _logger = LogManager.GetLogger(typeof(SignalConnector));

        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void setDomainId(String element)
        {
            _logger.Info("setDomainId:"+element);
        }
        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void setConnectorId(String element)
        {
            _logger.Info("setConnectorId:" + element);
        }
        /// <summary>
        /// Update Date Event
        /// </summary>
        /// <param name="arg0">Argument</param>
        /// <param name="arg1">Argument</param>
        /// <param name="arg2">Datas</param>
        public void updateData(string arg0, string arg1, object[] arg2)
        {            
            _logger.Info("updateData");
            foreach (Object obj in arg2)
            {
                _logger.Info(obj.ToString());
            }
        }

        /// <summary>
        /// Change Notification
        /// </summary>
        public void changeNotification()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get Alive state
        /// </summary>
        /// <param name="return">Alive State</param>
        /// <param name="returnSpecified">Is return Specified</param>
        public void getAliveState(out aliveState @return, out bool returnSpecified)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get Instance Id
        /// </summary>
        /// <returns>Instance id</returns>
        public string getInstanceId()
        {
            throw new NotImplementedException();
        }
    }
}
