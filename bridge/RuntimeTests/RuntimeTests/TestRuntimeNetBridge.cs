#region Copyright
// <copyright file="TestRuntimeNetBridge.cs" company="OpenEngSB">
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
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;
using RuntimeTests.TestConnectorImplementation;

namespace RuntimeTests.RuntimeTests
{
    public class TestRuntimeNetBridge : OSBRunTimeTestParent
    {
        #region Constructors
        private const String Destination = "tcp://localhost.:6549";
        private const String DomainName = "example";
        private const String Version = "3.0.0";
        #endregion
        #region Private Variables
        private ABridgeExceptionHandling exceptionHandler;
        private IDomainFactory factory;
        private String uuid;
        #endregion
        #region Public methods
        public override void CleanUp()
        {
            exceptionHandler.Stop = true;
            factory.StopAllConnections();
        }

        public override void Init()
        {
            exceptionHandler = new RetryDefaultExceptionHandler();
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", Destination, exampleDomain, exceptionHandler);
        }

        public void TestConnectToXlinkWithAConnectorThatIsNotregisteredConnectorAndAnalyseTheException()
        {
            uuid = factory.CreateDomainService("example");
            try
            {
                factory.ConnectToXLink(uuid, "localhost", "example", null);
            }
            catch (BridgeException ex)
            {
                Assert.AreEqual<String>(ex.Message, "The connecotr with id " + uuid + " has no instance");
            }
            finally
            {
                factory.DeleteDomainService(uuid);
            }
        }

        public void TestConnectToXlinkWithNoncreatedAndNonregisteredConnectorAndCheckTheCorrectException()
        {
            uuid = "exampleId";
            try
            {
                factory.ConnectToXLink(uuid, NullString, DomainName, null);
            }
            catch (BridgeException ex)
            {
                Assert.AreEqual<String>("The connecotr with id " + uuid + " has no instance", ex.Message);
            }
        }

        public void TestCreateDeleteConnectorAndNoRegistrationWorksCorrectly()
        {
            uuid = factory.CreateDomainService(DomainName);

            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(DomainName + "+external-connector-proxy+" + uuid));

            factory.DeleteDomainService(uuid);
        }

        public void TestCreateDeleteConnectorWithoutRegistrationWithUsernameAndPasswordWorksCorrectly()
        {
            uuid = factory.CreateDomainService(DomainName);

            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(DomainName + "+external-connector-proxy+" + uuid));

            factory.DeleteDomainService(uuid);
        }

        public void TestCreateMoreConnectorAndStopAllWorksCorrectly()
        {
            String[] uuids = new String[10];
            for (int i = 0; i < uuids.Length; i++)
            {
                uuid = factory.CreateDomainService(DomainName);
                uuids[i] = uuid;
                factory.RegisterConnector(uuid, DomainName);
                factory.UnRegisterConnector(uuid);
                factory.DeleteDomainService(uuid);
            }

            factory.StopAllConnections();
            foreach (String tmpuuid in uuids)
            {
                // Test if there is an connector that exists;
                try
                {
                    factory.GetDomainTypConnectorId(tmpuuid);
                    Assert.Fail();
                }
                catch (BridgeException ex)
                {
                    Assert.AreEqual<String>("There is no connector with the connectorId " + tmpuuid, ex.Message);
                }
            }
        }

        public void TestCreateRegisterCloseRegisterWorksCorrectly()
        {
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance(Version, Destination, exampleDomain, exceptionHandler);
            String tmpuuid = null;
            uuid = factory.CreateDomainService(DomainName);
            factory.RegisterConnector(uuid, DomainName);
            factory.StopConnection(uuid);
            tmpuuid = uuid;
            factory.RegisterConnector(uuid, DomainName);
            factory.StopConnection(uuid);
            Assert.AreEqual<String>(tmpuuid, uuid);
        }

        public void TestCreateRegisterConnectorAndGetEventHandlerWithInvalitConnectorId()
        {
            try
            {
                uuid = factory.RegisterConnector(NullString, DomainName);
                IExampleDomainEventsSoapBinding exampleDomain = factory.GetEventhandler<IExampleDomainEventsSoapBinding>(NullString);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
            }
        }

        public void TestCreateRegisterConnectXlinkDisconnectXlinkUnregisterDeleteConnectorOnADomainThatDoesNotSupportXlink()
        {
            uuid = factory.CreateDomainService(DomainName);
            factory.RegisterConnector(uuid, DomainName);

            try
            {
                // Exampledomain does not support xlink => should throw OpenEngSBException
                factory.ConnectToXLink(uuid, "localhost", "example", new OpenEngSBCore.ModelToViewsTuple[1]);
                Assert.Fail();
            }
            catch (OpenEngSBException)
            {
            }

            factory.UnRegisterConnector(uuid);
            factory.DeleteDomainService(uuid);
        }

        public void TestCreateRegisterUnregisterDeleteConnectorWorksCorrectly()
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

        public void TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectly()
        {
            uuid = factory.RegisterConnector(null, DomainName);
            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(DomainName + "+external-connector-proxy+" + uuid));
            factory.UnRegisterConnector(uuid);
            Assert.IsFalse(factory.Registered(uuid));
            factory.DeleteDomainService(uuid);
            Assert.IsFalse(factory.Registered(uuid));
        }

        public void TestCreateRegisterWithEventHandlerInvokedUnregisterDeleteWorksCorrectly()
        {
            LogEvent logEvent = new LogEvent();
            logEvent.level = "1";
            logEvent.message = "TestCase";
            logEvent.name = "Test";

            uuid = factory.RegisterConnector(null, DomainName);
            IExampleDomainEventsSoapBinding exampleDomain = factory.GetEventhandler<IExampleDomainEventsSoapBinding>(uuid);

            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(DomainName + "+external-connector-proxy+" + uuid));
            Assert.IsNotNull(exampleDomain);
            exampleDomain.raiseEvent(logEvent);
            factory.UnRegisterConnector(uuid);
            Assert.IsFalse(factory.Registered(uuid));
            factory.DeleteDomainService(uuid);
            Assert.IsFalse(factory.Registered(uuid));
        }
        #endregion
    }
}