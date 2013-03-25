using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using ExampleDomain;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using BridgeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestRuntimeNetBridgeWrongUsernameAndPassword
    {
        private IDomainFactory factory;
        private const String domainName = "example";
        private String uuid;
        private const String Username = "wrongAdmin";
        private const String Password = "wrongPassword";
        private const String destination = "tcp://localhost.:6549";
        private const String nullString = null;
        private const String version = "3.0.0";

        [TestInitialize]
        public void InitialiseFactory()
        {
            uuid = Guid.NewGuid().ToString();
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance(version, destination, exampleDomain, new ForwardDefaultExceptionHandler(), Username, Password);
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestCreateConnectorWithWrongUsernameAndPassword()
        {
            uuid = factory.CreateDomainService(domainName);
        }

        [ExpectedException(typeof(BridgeException))]
        public void TestRegisterConnectorWithoutCreateAndWithWrongUsernameAndPassword()
        {
            uuid = factory.RegisterConnector(uuid, domainName);
        }

        [TestCleanup]
        public void CleanUp()
        {
            factory.StopConnection(uuid);
        }
    }
}