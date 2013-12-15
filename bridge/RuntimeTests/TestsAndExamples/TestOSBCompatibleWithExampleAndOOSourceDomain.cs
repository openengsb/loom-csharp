#region Copyright
// <copyright file="TestOSBCompatibleWithExampleAndOOSourceDomain.cs" company="OpenEngSB">
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
using ExampleDomain;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300;
using RuntimeTests.TestConnectorImplementation;

namespace RuntimeTests.TestsAndExamples
{
    public class TestOSBCompatibleWithExampleAndOOSourceDomain : OSBRunTimeTestParent
    {
        #region Public Methods
        public override void CleanUp()
        {
        }

        public override void Init()
        {
        }

        public void TestExampleCodeExecution()
        {
            Boolean xlink = false;
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            OOSourceCodeDomainConnector ooconnector = new OOSourceCodeDomainConnector();
            OpenEngSB300.SetSupport();
            IDomainFactory factory;
            string domainName;
            string destination = "tcp://localhost.:6549";
            if (xlink)
            {
                // if you are using xlink for the example, please use an other domain. Example domain is not linkable
                domainName = "oosourcecode";
                factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, ooconnector, new RetryDefaultExceptionHandler());
            }
            else
            {
                domainName = "example";
                factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, exampleDomain, new ForwardDefaultExceptionHandler());
            }

            String connectorId = factory.CreateDomainService(domainName);
            factory.RegisterConnector(connectorId, domainName);
            if (xlink)
            {
                XLinkUrlBlueprint template = factory.ConnectToXLink(connectorId, "localhost", domainName, InitModelViewRelation());
                factory.DisconnectFromXLink(connectorId, "localhost");
            }
            else
            {
                IExampleDomainEventsSoapBinding remotedomain = factory.GetEventhandler<IExampleDomainEventsSoapBinding>(connectorId);
                LogEvent logEvent = new LogEvent();
                logEvent.name = "Example";
                logEvent.level = "DEBUG";
                logEvent.message = "remoteTestEventLog";
                remotedomain.raiseEvent(logEvent);
            }

            factory.UnRegisterConnector(connectorId);
            factory.DeleteDomainService(connectorId);
            factory.StopConnection(connectorId);
        }

        private ModelToViewsTuple[] InitModelViewRelation()
        {
            ModelToViewsTuple[] modelsToViews = new ModelToViewsTuple[1];
            Dictionary<String, String> descriptions = new Dictionary<String, String>();
            descriptions.Add("en", "This view opens the values in a SQLViewer.");
            descriptions.Add("de", "Dieses Tool öffnet die Werte in einem SQLViewer.");
            XLinkConnectorView[] views = new XLinkConnectorView[1];
            views[0] = new XLinkConnectorView()
            {
                name = "SQLView",
                viewId = "SQL Viewer",
                descriptions = descriptions.ConvertMap<string2stringMapEntry>()
            };
            modelsToViews[0] =
                new ModelToViewsTuple()
            {
                description = new ModelDescription() { modelClassName = "org.openengsb.domain.SQLCode.model.SQLCreate", versionString = "3.0.0.SNAPSHOT" },
                views = views
            };
            return modelsToViews;
        }
        #endregion
    }
}