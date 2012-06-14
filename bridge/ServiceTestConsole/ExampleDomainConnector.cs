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
using Implementation;
namespace ServiceTestConsole
{
    /// <summary>
    /// Example implementation of the local domain
    /// </summary>
    class ExampleDomainConnector : RegistrationFunctions,IExampleDomainSoap11Binding
    {
        private static ILog logger = LogManager.GetLogger(typeof(ExampleDomainConnector));
        
        public ExampleDomainConnector() : base(logger)
        {
        }
        /*public string doSomethingWithEnum(exampleEnum arg0, bool arg0Specified)
        {
            logger.Info("run doSomethingWithEnum with " + arg0);
            return "done";
        }

        public string doSomethingWithLogEvent(logEvent arg0)
        {
            logger.Info("run doSomethingWithLogEvent with " + arg0);
            return "done";
        }

        public string doSomethingWithMessage(string arg0)
        {
            logger.Info("run doSomethingWithMessage with " + arg0);
            return "done";
        }

        public object doSomethingWithModel(object arg0)
        {
            logger.Info("run doSomethingWithModel with " + arg0);
            return null;
        }

        public void getAliveState(out ExampleDomain.aliveState @return, out bool returnSpecified)
        {
            throw new NotImplementedException();
        }

        public string getInstanceId()
        {
            throw new NotImplementedException();
        }*/

        public string doSomethingWithEnum(ExampleDomain_ExampleEnum args0)
        {
            throw new NotImplementedException();
        }

        public string doSomethingWithLogEvent(LogEvent args0)
        {
            throw new NotImplementedException();
        }

        public string doSomethingWithMessage(string args0)
        {
            logger.Info("run doSomethingWithEnum with " + args0);
            return "Hallo";
        }

        public ExampleResponseModel doSomethingWithModel(ExampleRequestModel args0)
        {            
            logger.Info("Message received in doSomethingWithModel");
            ExampleResponseModel m=new ExampleResponseModel();
            m.result = "RESULT";
            return m;
        }
    }
}
