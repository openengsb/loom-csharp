using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using System.Net;
using System.Diagnostics.CodeAnalysis;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestDomainFactoryProvider
    {
        private const String version300 = "3.0.0";
        private const String version240 = "2.4.0";
        private const String versionInvalid = "1.4.0";
        private const String username = "admin";
        private const String password = "password";
        private const String nullString = null;
        private const ABridgeExceptionHandling nullExceptionHandler = null;

        [TestMethod]
        public void TestFactoryReturnsConnector300OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version300, nullString, nullString);

            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsConnector240OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version240, nullString, nullString);

            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsNullWithInvalidVersionNumberAsParameter()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(versionInvalid, nullString, nullString);
            
            Assert.IsNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsNullWithInvalidVersionNumberAsParameterAndExceptionHandlerAsParameter()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(versionInvalid, nullString, nullString, nullExceptionHandler);

            Assert.IsNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsConnector300OpenEngSBVersionWithExceptionHandlerParameter()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version300, nullString, nullString, nullExceptionHandler);
            
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsConnector240OpenEngSBVersionWithExceptionHandlerParameter()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version240, nullString, nullString, nullExceptionHandler);
            
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void TestFactoryReturnsConnector300OpenEngSBVersionWithUsernameAndPassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version300, nullString, nullString, username, password);
            
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsConnector240OpenEngSBVersionWithUsernameAndPassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version240, nullString, nullString, username, password);
            
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsNullWithInvalidVersionNumberAsParameterAndUsernamPassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(versionInvalid, nullString, nullString, username, password);

            Assert.IsNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsNullWithInvalidVersionNumberAsParameterAndExceptionHandlerUsernamPassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(versionInvalid, nullString, nullString, nullExceptionHandler, username, password);

            Assert.IsNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsConnector300OpenEngSBVersionWithUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version300, nullString, nullString, nullExceptionHandler, username, password);
            
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryReturnsConnector240OpenEngSBVersionWithUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version240, nullString, nullString, nullExceptionHandler, username, password);
            
            Assert.IsNotNull(factory);
        }
    }
}