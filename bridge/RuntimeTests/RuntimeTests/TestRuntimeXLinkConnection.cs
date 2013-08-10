#region Copyright
// <copyright file="TestRuntimeXLinkConnection.cs" company="OpenEngSB">
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
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOSourceCodeDomain;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using RuntimeTests.TestConnectorImplementation;

namespace RuntimeTests.RuntimeTests
{
    public class TestRuntimeXLinkConnection : OSBRunTimeTestParent
    {
        #region Constants
        private const String Destination = "tcp://localhost.:6549";
        private const String DomainName = "oosourcecode";
        #endregion
        #region Private Variables
        private IDomainFactory factory;
        private String uuid;
        #endregion
        #region Public Methods
        public override void CleanUp()
        {
            factory.StopConnection(uuid);
        }

        public override void Init()
        {
            IOOSourceCodeDomainSoapBinding exampleDomain = new OOSourceCodeDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", Destination, exampleDomain, new ForwardDefaultExceptionHandler());
        }

        public void TestCreateRegisterConnecttoxlinkDisconnectfromXlinkUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectly()
        {
            uuid = factory.RegisterConnector(null, DomainName);

            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(DomainName + "+external-connector-proxy+" + uuid));

            XLinkUrlBlueprint template = factory.ConnectToXLink(uuid, "localhost", DomainName, InitModelViewRelation());
            factory.DisconnectFromXLink(uuid, "localhost");
            factory.UnRegisterConnector(uuid);

            Assert.IsFalse(factory.Registered(uuid));

            factory.DeleteDomainService(uuid);

            Assert.IsFalse(factory.Registered(uuid));
        }

        private ModelToViewsTuple[] InitModelViewRelation()
        {
            ModelToViewsTuple[] modelsToViews = new ModelToViewsTuple[1];
            Dictionary<String, String> descriptions = new Dictionary<String, String>();
            descriptions.Add("en", "This view opens the values in a SQLViewer.");
            descriptions.Add("de", "Dieses Tool öffnet die Werte in einem SQLViewer.");
            OpenEngSBCore.XLinkConnectorView[] views = new OpenEngSBCore.XLinkConnectorView[1];
            views[0] = new OpenEngSBCore.XLinkConnectorView()
            {
                name = "SQLView",
                viewId = "SQL Viewer",
                descriptions = descriptions.ConvertMap<OpenEngSBCore.string2stringMapEntry>()
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