using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using BridgeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;

namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestRegisterConnectorWithHasBeenCreated
    {
        [TestMethod]
        public void TestCreateRegisterCloseRegister()
        {
            string destination = "tcp://localhost.:6549";
            String domainName = "example";
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, exampleDomain, new RetryDefaultExceptionHandler());
            String uuid = factory.CreateDomainService(domainName);
            factory.RegisterConnector(uuid, domainName);
            factory.StopConnection(uuid);
            factory.RegisterConnector(uuid, domainName);
            factory.StopConnection(uuid);
        }
        [TestMethod]
        public void TestRegisterClose()
        {
            string destination = "tcp://localhost.:6549";
            String domainName = "example";
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, exampleDomain, new RetryDefaultExceptionHandler());
            String uuid = factory.RegisterConnector(Guid.NewGuid().ToString(), domainName);
            factory.StopConnection(uuid);
            factory.RegisterConnector(uuid, domainName);
        }

    }
}