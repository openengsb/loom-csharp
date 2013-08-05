using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using ExampleDomain;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using RuntimeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using OSBConnection;
using SonaTypeDependencies;
namespace RuntimeTests.RuntimeTests
{

    public class TestRuntimeNetBridgeUserNameAndPassword : OSBRunTimeTestParent
    {
        private IDomainFactory factory;
        private const String domainName = "example";
        private String uuid;
        private const String Username = "admin";
        private const String Password = "password";
        private const String destination = "tcp://localhost.:6549";
        private const String nullString = null;
        private const String version = "3.0.0";

        public override void Init()
        {
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance(version, destination, exampleDomain, new ForwardDefaultExceptionHandler(), Username, Password);
        }

        
        public void TestCreateDeleteConnectorAndNoRegistrationWorksCorrectlyWithUsernameAndPassword()
        {
            uuid = factory.CreateDomainService(domainName);

            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));

            factory.DeleteDomainService(uuid);
        }
        
        public void TestCreateDeleteConnectorWithoutRegistrationWithUsernameAndPasswordWorksCorrectlyUsernameAndPassword()
        {
            uuid = factory.CreateDomainService(domainName);

            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));

            factory.DeleteDomainService(uuid);
        }

        public void TestCreateRegisterUnregisterDeleteConnectorWorksCorrectlyWithUsernameAndPassword()
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

        
        public void TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectlyWithUsernameAndPassword()
        {
            uuid = factory.RegisterConnector(null, domainName);
            factory.UnRegisterConnector(uuid);
            factory.DeleteDomainService(uuid);
        }

        
        public void TestCreateRegisterEventHandlerUnregisterDeleteWorksCorrectlyWithUsernameAndPassword()
        {
            uuid = factory.RegisterConnector(nullString, domainName);

            IExampleDomainEventsSoapBinding exampleDomain = factory.GetEventhandler<IExampleDomainEventsSoapBinding>(uuid);

            factory.UnRegisterConnector(uuid);
            factory.DeleteDomainService(uuid);
        }

        public override void CleanUp()
        {
            factory.StopAllConnections();
        }
    }
}