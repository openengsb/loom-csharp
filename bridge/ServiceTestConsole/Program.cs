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
using System.Reflection;
using System.IO;
using Bridge.Implementation;
using Bridge.Interface;
using ExampleDomain;
using ExampleDomainEvents;

namespace Bridge.ServiceTestConsole
{
    class Program
    {
        /// <summary>
        /// This version works with the OpenEngS 3.0.0-Snapshot Framwork
        /// </summary>
        /// <param name="args">System Arguments</param>
        static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();

            string destination = "tcp://localhost.:6549";
            string domainName = "example";
            Type tessss = typeof(ExampleDomain.ExampleEnum);
            String test = typeof(ExampleDomain.ExampleEnum).ToString();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0");
            IExampleDomainSoap11Binding localDomain = new ExampleDomainConnector();

            //Register the connecter on the osenEngSB
            factory.RegisterDomainService(destination, localDomain, domainName);
            //Get a remote handler, to raise events on obenEngSB
            IExampleDomainEventsSoap11Binding remotedomain = factory.getEventhandler<IExampleDomainEventsSoap11Binding>(destination);
            ExampleDomainEvents.LogEvent logEvent = new ExampleDomainEvents.LogEvent();
            logEvent.name = "Example";
            logEvent.processId = 0;
            logEvent.level = ExampleDomainEvents.LogLevel.DEBUG;
            logEvent.message = "remoteTestEventLog";
            remotedomain.raiseEvent(logEvent);
            Console.ReadKey();
            factory.UnregisterDomainService(localDomain);            
        }
    }
}
