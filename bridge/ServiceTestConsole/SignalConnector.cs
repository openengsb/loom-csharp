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
using TestDomain;
namespace Org.OpenEngSB.Loom.Csharp.Common.ServiceTestConsole
{

    /// <summary>
    /// Example implementation of the local domain
    /// </summary>
    class TestDomainConnector : ITestDomainSoap11Binding
    {
        private ILog logger = LogManager.GetLogger(typeof(TestDomainConnector));

        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void setDomainId(String element)
        {
            logger.Info("setDomainId:" + element);
        }
        public void setConnectorId(String element)
        {
            logger.Info("setConnectorId:" + element);
        }
        public string runTests()
        {
            logger.Info("run test simulation");
            return "done";
        }
        public void runTestsProcessId(long args0, bool args0Specified)
        {
            logger.Info("run test "+args0+ " "+args0Specified);            
        }
    }
}
