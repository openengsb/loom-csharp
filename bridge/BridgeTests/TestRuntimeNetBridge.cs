using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using ExampleDomain;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using BridgeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;
namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestRuntimeNetBridge
    {
        IDomainFactory factory;
        String domainName;
        String uuid;
        [TestInitialize]
        public void InitialiseFactory()
        {
            string destination = "tcp://localhost.:6549";
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, exampleDomain, new RetryDefaultExceptionHandler());
        }
        [TestMethod]
        public void TestCreateDeleteConnector()
        {
            domainName = "example";
            uuid = factory.CreateDomainService(domainName);
            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            factory.DeleteDomainService(uuid);
        }
        [TestMethod]
        public void TestCreateDeleteConnectorWithUsernameAndPassword()
        {
            domainName = "example";
            uuid = factory.CreateDomainService(domainName);
            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            factory.DeleteDomainService(uuid);
        }
        [TestMethod]
        public void TestCreateRegisterUnregisterDeleteConnector()
        {
            domainName = "example";
            uuid = factory.CreateDomainService(domainName);
            factory.RegisterConnector(uuid, domainName);
            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            factory.UnRegisterConnector(uuid);
            Assert.IsFalse(factory.Registered(uuid));
            factory.DeleteDomainService(uuid);
            Assert.IsFalse(factory.Registered(uuid));
        }
        [TestMethod]
        public void TestCreateRegisterConnectXlinkDisconnectXlinkUnregisterDeleteConnector()
        {
            domainName = "example";
            uuid = factory.CreateDomainService(domainName);
            factory.RegisterConnector(uuid, domainName);
            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            try
            {
                //Exampledomain does not support xlink => should throw OpenEngSBException
                factory.ConnectToXLink(uuid, "localhost", "example", new OpenEngSBCore.ModelToViewsTuple[1]);
            }
            catch (OpenEngSBException)
            {
            }
            factory.UnRegisterConnector(uuid);
            Assert.IsFalse(factory.Registered(uuid));
            factory.DeleteDomainService(uuid);
            Assert.IsFalse(factory.Registered(uuid));
        }
        [TestMethod]
        public void TestInvalideStates()
        {
            uuid = "exampleId";
            try
            {
                factory.ConnectToXLink("exampleId", "localhost", "example", null);
            }
            catch (BridgeException ex)
            {
                Assert.AreEqual("The connecotr with id " + uuid + " has no instance (Invokde createDomainService)", ex.Message);
            }

            try
            {
                uuid = factory.CreateDomainService("example");
                factory.ConnectToXLink(uuid, "localhost", "example", null);
            }
            catch (BridgeException ex)
            {
                Assert.AreEqual<String>(ex.Message, "The connecotr with id " + uuid + " is not registered");
            }

        }

        [TestMethod]
        public void TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnector()
        {
            domainName = "example";
            uuid = factory.RegisterConnector(null, domainName);
            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            factory.UnRegisterConnector(uuid);
            Assert.IsFalse(factory.Registered(uuid));
            factory.DeleteDomainService(uuid);
            Assert.IsFalse(factory.Registered(uuid));
        }
        [TestMethod]
        public void TestCreateRegisterEventHandlerInvokeUnregisterDelete()
        {
            domainName = "example";
            uuid = factory.RegisterConnector(null, domainName);
            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            IExampleDomainEventsSoap11Binding exampleDomain = factory.GetEventhandler<IExampleDomainEventsSoap11Binding>(uuid);
            Assert.IsNotNull(exampleDomain);
            LogEvent logEvent = new LogEvent();
            logEvent.level = "1";
            logEvent.message = "TestCase";
            logEvent.name = "Test";
            exampleDomain.raiseEvent(logEvent);
            try
            {
                exampleDomain.raiseEvent(null);
            }
            catch (BridgeException)
            {
            }
            factory.UnRegisterConnector(uuid);
            Assert.IsFalse(factory.Registered(uuid));
            factory.DeleteDomainService(uuid);
            Assert.IsFalse(factory.Registered(uuid));
        }
        [TestMethod]
        public void TestCreateMoreConnectorAndStopAll()
        {
            domainName = "example";
            String[] uuids = new String[10];
            for (int i = 0; i < uuids.Length; i++)
            {

                uuid = factory.CreateDomainService(domainName);
                uuids[i] = uuid;
                Assert.IsFalse(factory.Registered(uuid));
                factory.RegisterConnector(uuid, domainName);
                Assert.IsTrue(factory.Registered(uuid));
                Assert.IsFalse(factory.Registered("WRONG ID"));
                Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
                factory.UnRegisterConnector(uuid);
                Assert.IsFalse(factory.Registered(uuid));
                factory.DeleteDomainService(uuid);
                Assert.IsFalse(factory.Registered(uuid));
            }
            factory.StopAllConnections();
            foreach (String tmpuuid in uuids)
            {
                try
                {
                    factory.GetDomainTypConnectorId(tmpuuid);
                }
                catch (BridgeException ex)
                {
                    Assert.AreEqual<String>("There is no connector with the connectorId " + tmpuuid, ex.Message);
                }
            }
        }
        [TestCleanup]
        public void CleanUp()
        {
            factory.StopAllConnections();
        }
    }
}