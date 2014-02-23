#region Copyright
// <copyright file="TestRuntimeNetBridgeUserNameAndPassword.cs" company="OpenEngSB">
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
using ExampleDomain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using RuntimeTests.TestConnectorImplementation;

namespace RuntimeTests.RuntimeTests
{
    public class TestRuntimeNetBridgeUserNameAndPassword : OSBRunTimeTestParent
    {
        #region Constants
        private const String Destination = "tcp://localhost.:6549";
        private const String DomainName = "example";
        private const String Password = "password";
        private const String Username = "admin";
        private const String Version = "3.0.0";
        #endregion
        #region Private Variables
        private IDomainFactory factory;
        private String uuid;
        #endregion
        #region Public Methods
        public override void CleanUp()
        {
            factory.StopAllConnections();
        }

        public override void Init()
        {
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance(Version, Destination, exampleDomain, new ForwardDefaultExceptionHandler(), Username, Password);
        }

        public void TestCreateDeleteConnectorAndNoRegistrationWorksCorrectlyWithUsernameAndPassword()
        {
            uuid = factory.CreateDomainService(DomainName);

            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(DomainName + "+external-connector-proxy+" + uuid));

            factory.DeleteDomainService(uuid);
        }

        public void TestCreateDeleteConnectorWithoutRegistrationWithUsernameAndPasswordWorksCorrectlyUsernameAndPassword()
        {
            uuid = factory.CreateDomainService(DomainName);

            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(DomainName + "+external-connector-proxy+" + uuid));

            factory.DeleteDomainService(uuid);
        }

        public void TestCreateRegisterEventHandlerUnregisterDeleteWorksCorrectlyWithUsernameAndPassword()
        {
            uuid = factory.RegisterConnector(NullString, DomainName);

            IExampleDomainEventsSoapBinding exampleDomain = factory.GetEventhandler<IExampleDomainEventsSoapBinding>(uuid);

            factory.UnRegisterConnector(uuid);
            factory.DeleteDomainService(uuid);
        }

        public void TestCreateRegisterUnregisterDeleteConnectorWorksCorrectlyWithUsernameAndPassword()
        {
            uuid = factory.CreateDomainService(DomainName);
            factory.RegisterConnector(uuid, DomainName);

            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(DomainName + "+external-connector-proxy+" + uuid));

            factory.UnRegisterConnector(uuid);

            Assert.IsFalse(factory.Registered(uuid));

            factory.DeleteDomainService(uuid);

            Assert.IsFalse(factory.Registered(uuid));
        }

        public void TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectlyWithUsernameAndPassword()
        {
            uuid = factory.RegisterConnector(null, DomainName);
            factory.UnRegisterConnector(uuid);
            factory.DeleteDomainService(uuid);
        }
        #endregion
    }
}