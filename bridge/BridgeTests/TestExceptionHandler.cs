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
        private const String domainName = "example";
        private const String version = "3.0.0";
        private const String nullString = null;

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void TestForwardExceptionHandlerForwardsTheException()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance(version, nullString, new ExampleDomainConnector(), new ForwardDefaultExceptionHandler());
            factory.CreateDomainService(domainName);
        }

        [Ignore]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRetryExceptionHandlerRetiesTheMethod()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance(version, "FooBar", new ExampleDomainConnector(), new RetryDefaultExceptionHandler());
            //Visual Studio unit tests does not work, when a threads are used.
            factory.StopAllConnections();
            factory.CreateDomainService(domainName);
        }
    }
}