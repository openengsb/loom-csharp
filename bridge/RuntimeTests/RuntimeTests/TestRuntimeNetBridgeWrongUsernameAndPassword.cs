#region Copyright
// <copyright file="TestRuntimeNetBridgeWrongUsernameAndPassword.cs" company="OpenEngSB">
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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using RuntimeTests.TestConnectorImplementation;

namespace RuntimeTests.RuntimeTests
{
    public class TestRuntimeNetBridgeWrongUsernameAndPassword : OSBRunTimeTestParent
    {
        #region Constants
        private const String Destination = "tcp://localhost.:6549";
        private const String DomainName = "example";
        private const String Password = "wrongPassword";
        private const String Username = "wrongAdmin";
        private const String Version = "3.0.0";
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
            uuid = Guid.NewGuid().ToString();
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance(Version, Destination, exampleDomain, new ForwardDefaultExceptionHandler(), Username, Password);
        }

        public void TestCreateConnectorWithWrongUsernameAndPassword()
        {
            try
            {
                uuid = factory.CreateDomainService(DomainName);
                Assert.Fail();
            }
            catch (BridgeException)
            {
            }
        }

        public void TestRegisterConnectorWithoutCreateAndWithWrongUsernameAndPassword()
        {
            try
            {
                uuid = factory.RegisterConnector(uuid, DomainName);
                Assert.Fail();
            }
            catch (BridgeException)
            {
            }
        }
        #endregion
    }
}