using System;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using ExampleDomain;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using RuntimeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RuntimeTests.RuntimeTests
{

    public class TestRuntimeNetBridgeWrongUsernameAndPassword : OSBRunTimeTestParent
    {
        private IDomainFactory factory;
        private const String domainName = "example";
        private String uuid;
        private const String Username = "wrongAdmin";
        private const String Password = "wrongPassword";
        private const String destination = "tcp://localhost.:6549";
        private const String nullString = null;
        private const String version = "3.0.0";
    
        public override void Init()
        {
            uuid = Guid.NewGuid().ToString();
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance(version, destination, exampleDomain, new ForwardDefaultExceptionHandler(), Username, Password);
        }

        public void TestCreateConnectorWithWrongUsernameAndPassword()
        {
            try
            {
                uuid = factory.CreateDomainService(domainName);
                Assert.Fail();
            }
            catch (BridgeException) { }
        }
        public void TestRegisterConnectorWithoutCreateAndWithWrongUsernameAndPassword()
        {
            try
            {
                uuid = factory.RegisterConnector(uuid, domainName);
                Assert.Fail();
            }
            catch (BridgeException) { }
        }

        public override void CleanUp()
        {
            factory.StopConnection(uuid);
        }
    }
}