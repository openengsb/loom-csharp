using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using BridgeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;

namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestCloseAndRegisterConnector
    {
        [TestMethod]
        public void TestRegisterFunktionInvocation()
        {
            ExampleDomainConnector connector = new ExampleDomainConnector();
            connector.SetDomainId("TestCase");
            connector.SetConnectorId("TastConenctorId");
            
            Assert.IsTrue(connector is RegistrationFunctions);
            Assert.AreEqual<AliveState>(AliveState.ONLINE, connector.GetAliveState());
        }
    }
}