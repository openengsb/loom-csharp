using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using System.Threading;
using BridgeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;

namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestExceptionHandler
    {
        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void TestForwardExceptionHandler()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", null, new ExampleDomainConnector(), new ForwardDefaultExceptionHandler());
            factory.CreateDomainService("example");
        }
        [Ignore]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRetryExceptionHandler()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", "ASD", new ExampleDomainConnector(), new RetryDefaultExceptionHandler());
            //VIsual Studio unit tests does not work, when a threads are used.
            factory.StopAllConnections();
            factory.CreateDomainService("example");
        }
    }
}