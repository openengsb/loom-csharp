using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using System.Net;
using System.Diagnostics.CodeAnalysis;

namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestDomainFactoryProvider
    {
        [TestMethod]
        public void TestFactory300OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("3.0.0", null, null);
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactory240OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("2.4.0", null, null);
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryInvalidOpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("1.4.0", null, null);
            Assert.IsNull(factory);
            factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("Test", null, null);
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryExceptionHanderInvalidOpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("1.4.0", null, null, null);
            Assert.IsNull(factory);
            factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("Test", null, null, null);
        }
        [TestMethod]
        public void TestFactoryWithExceptionHandler300OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("3.0.0", null, null, null);
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryWithExceptionHandler240OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("2.4.0", null, null, null);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void TestFactory300OpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("3.0.0", null, null, "admin", "password");
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactory240OpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("2.4.0", null, null, "admin", "password");
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryInvalidOpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("1.4.0", null, null, "admin", "password");
            Assert.IsNull(factory);
            factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("Test", null, null, "admin", "password");
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryExceptionHanderInvalidOpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("1.4.0", null, null, null, "admin", "password");
            Assert.IsNull(factory);
            factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("Test", null, null, null, "admin", "password");
        }
        [TestMethod]
        public void TestFactoryWithExceptionHandler300OpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("3.0.0", null, null, null, "admin", "password");
            Assert.IsNotNull(factory);
        }
        [TestMethod]
        public void TestFactoryWithExceptionHandler240OpenEngSBVersionUsernamePassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>("2.4.0", null, null, null, "admin", "password");
            Assert.IsNotNull(factory);
        }
    }
}