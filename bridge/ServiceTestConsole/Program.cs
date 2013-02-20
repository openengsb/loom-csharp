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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using log4net;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using System.Collections.Generic;
using ExampleDomain;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using OpenEngSBCore;

namespace ServiceTestConsole
{
    class Program
    {
        /// <summary>
        /// This version works with the OpenEngS 3.0.0-Snapshot Framework
        /// </summary>
        /// <param name="args">System Arguments</param>
        static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();
            ILog logger;
            Boolean xlink = true;
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            OOSourceCodeDomainConnector ooconnector = new OOSourceCodeDomainConnector();
            IDomainFactory factory;
            string domainName;
            string destination = "tcp://localhost.:6549";
            if (xlink)
            {
                //if you are using xlink for the example, please use an other domain. Example domain is not linkable
                domainName = "oosourcecode";
                logger = LogManager.GetLogger(typeof(OOSourceCodeDomainConnector));
                factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, exampleDomain, new RetryDefaultExceptionHandler());
            }
            else
            {
                domainName = "example";
                logger = LogManager.GetLogger(typeof(ExampleDomainConnector));
                factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, ooconnector, new RetryDefaultExceptionHandler());
            }

            logger.Info("Start Example wit the domain " + domainName);

            String serviceId = factory.CreateDomainService(domainName);
            factory.RegisterConnector(serviceId, domainName);
            if (xlink)
            {
                XLinkUrlBlueprint template = factory.ConnectToXLink(domainName, initModelViewRelation());
                factory.DisconnectFromXLink(domainName);
            }
            else
            {
                IExampleDomainEventsSoap11Binding remotedomain = factory.getEventhandler<IExampleDomainEventsSoap11Binding>(domainName);
                LogEvent lEvent = new LogEvent();
                lEvent.name = "Example";
                lEvent.level = "DEBUG";
                lEvent.message = "remoteTestEventLog";
                remotedomain.raiseEvent(lEvent);
            }
            logger.Info("Press enter to close the Connection");
            Console.ReadKey();
            factory.UnRegisterConnector(domainName);
            factory.DeleteDomainService(domainName);
            factory.StopConnection(domainName);
        }


        private static ModelToViewsTuple[] initModelViewRelation()
        {
            ModelToViewsTuple[] modelsToViews = new ModelToViewsTuple[1];
            Dictionary<String, String> descriptions = new Dictionary<String, String>();
            descriptions.Add("en", "This view opens the values in a SQLViewer.");
            descriptions.Add("de", "Dieses Tool öffnet die Werte in einem SQLViewer.");

            XLinkConnectorView[] views = new XLinkConnectorView[1];
            views[0] = (new XLinkConnectorView() { name = "SQLView", viewId = "SQL Viewer", descriptions = descriptions.ConvertMap<entry3>() });
            modelsToViews[0] =
                    new ModelToViewsTuple()
                    {
                        description = new ModelDescription() { modelClassName = "org.openengsb.domain.SQLCode.model.SQLCreate", versionString = "3.0.0.SNAPSHOT" },
                        views = views
                    };
            return modelsToViews;
        }
    }
}