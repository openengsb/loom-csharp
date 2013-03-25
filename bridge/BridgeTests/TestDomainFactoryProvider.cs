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
        public void TestFactory300OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version300, nullString, nullString);

            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactory240OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version240, nullString, nullString);

            Assert.IsNotNull(factory);
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryInvalidOpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(versionInvalid, nullString, nullString);
            
            Assert.IsNull(factory);
            factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("Test", nullString, nullString);
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryExceptionHanderInvalidOpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(versionInvalid, nullString, nullString, nullExceptionHandler);
            
            Assert.IsNull(factory);
            factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("Test", nullString, nullString, nullExceptionHandler);
        }
        [TestMethod]
        public void TestFactoryWithExceptionHandler300OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version300, nullString, nullString, nullExceptionHandler);
            
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryWithExceptionHandler240OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version240, nullString, nullString, nullExceptionHandler);
            
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void TestFactory300OpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version300, nullString, nullString, username, password);
            
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactory240OpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version240, nullString, nullString, username, password);
            
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryInvalidOpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(versionInvalid, nullString, nullString, username, password);
            
            Assert.IsNull(factory);
            factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("Test", nullString, nullString, username, password);
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryExceptionHanderInvalidOpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(versionInvalid, nullString, nullString, nullExceptionHandler, username, password);
            
            Assert.IsNull(factory);
            factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("Test", nullString, nullString, nullExceptionHandler, username, password);
        }
        [TestMethod]
        public void TestFactoryWithExceptionHandler300OpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version300, nullString, nullString, nullExceptionHandler, username, password);
            
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryWithExceptionHandler240OpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(version240, nullString, nullString, nullExceptionHandler, username, password);
            
            Assert.IsNotNull(factory);
        }
    }
}