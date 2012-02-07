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
using ExampleDomain;
namespace Org.OpenEngSB.Loom.Csharp.Common.ServiceTestConsole
{

    /// <summary>
    /// Example implementation of the local domain
    /// </summary>
    class ExampleDomainConnector : IExampleDomainSoapBinding
    {
        private ILog _logger = LogManager.GetLogger(typeof(ExampleDomainConnector));

        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void setDomainId(String element)
        {
            _logger.Info("setDomainId:" + element);
        }

        public void setConnectorId(String element)
        {
            _logger.Info("setConnectorId:" + element);
        }
        public string doSomething(string arg0)
        {
            _logger.Info("Result doSomething: " + arg0);
            return "received";
        }

        public string doSomething1(exampleEnum arg0, bool arg0Specified)
        {
            _logger.Info("Result doSomething1: " + arg0+" "+arg0Specified);
            return "received";
        }

        public object doSomething2(object arg0)
        {
            _logger.Info("Result doSomething2: " + arg0);
            return null;
        }

        public string doSomethingWithLogEvent(logEvent arg0)
        {
            _logger.Info("Result doSomething: " + arg0);
            return "received";
        }

        public void getAliveState(out aliveState @return, out bool returnSpecified)
        {
            throw new NotImplementedException();
        }

        public string getInstanceId()
        {
            throw new NotImplementedException();
        }
    }
}
