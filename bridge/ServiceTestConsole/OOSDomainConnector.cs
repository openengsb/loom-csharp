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
using OOSourceCodeDomain;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;

using log4net;
namespace ServiceTestConsole
{
    /// <summary>
    /// Example implementation of the local domain
    /// </summary>
    public class OOSourceCodeDomainConnector : RegistrationFunctions, IOOSourceCodeDomainSoap11Binding
    {
        private static ILog logger = LogManager.GetLogger(typeof(ExampleDomainConnector));

        public OOSourceCodeDomainConnector() : base(logger) { }


        public void updateClass(OOClass args0)
        {
            throw new NotImplementedException();
        }

        public void getAliveState(out orgopenengsbcoreapiAliveState? @return, out bool returnSpecified)
        {
            throw new NotImplementedException();
        }

        public string getInstanceId()
        {
            throw new NotImplementedException();
        }

        public void onRegisteredToolsChanged(XLinkConnector[] args0)
        {
            throw new NotImplementedException();
        }

        public void openXLinks(object[] args0, string args1)
        {
            throw new NotImplementedException();
        }
    }
}