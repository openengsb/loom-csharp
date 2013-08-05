using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using BridgeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;

namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestCloseAndRegisterConnector
    {
        [TestMethod]
        public void TestIfTheDefaultValuesInRegisterFuntionIsCorrect()
        {
            ExampleDomainConnector connector = new ExampleDomainConnector();
            connector.SetDomainId("TestCase");
            connector.SetConnectorId("TastConenctorId");
            
            Assert.IsTrue(connector is RegistrationFunctions);
            Assert.AreEqual<AliveState>(AliveState.ONLINE, connector.GetAliveState());
        }
    }
}