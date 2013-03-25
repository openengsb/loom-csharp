using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using ExampleDomain;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using BridgeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;
namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestRuntimeNetBridgeUserNameAndPassword
    {
        private IDomainFactory factory;
        private const String domainName = "example";
        private String uuid;
        private const String Username = "admin";
        private const String Password = "password";
        private const String destination = "tcp://localhost.:6549";
        [TestInitialize]
        public void InitialiseFactory()
        {            
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, exampleDomain, new ForwardDefaultExceptionHandler(), Username, Password);
        }
        [TestMethod]
        public void TestCreateDeleteConnector()
        {
            uuid = factory.CreateDomainService(domainName);
            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            factory.DeleteDomainService(uuid);
        }
        [TestMethod]
        public void TestCreateDeleteConnectorWithUsernameAndPassword()
        {
            uuid = factory.CreateDomainService(domainName);
            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            factory.DeleteDomainService(uuid);
        }
        [TestMethod]
        public void TestCreateRegisterUnregisterDeleteConnector()
        {
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
        public void TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnector()
        {
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
        public void TestCreateRegisterEventHandlerUnregisterDelete()
        {
            uuid = factory.RegisterConnector(null, domainName);
            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            IExampleDomainEventsSoap11Binding exampleDomain = factory.GetEventhandler<IExampleDomainEventsSoap11Binding>(uuid);
            Assert.IsNotNull(exampleDomain);
            factory.UnRegisterConnector(uuid);
            Assert.IsFalse(factory.Registered(uuid));
            factory.DeleteDomainService(uuid);
            Assert.IsFalse(factory.Registered(uuid));
        }
        [TestCleanup]
        public void CleanUp()
        {
            factory.StopConnection(uuid);
        }
    }
}