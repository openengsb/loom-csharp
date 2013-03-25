using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestDestination
    {
        private const String url = "localhost:8888";
        [TestMethod]
        public void TestDestinationWithoutQueue()
        {
            Destination destination = new Destination(url + "?");

            Assert.AreEqual<String>(destination.Host, url);
            Assert.IsTrue(String.IsNullOrEmpty(destination.Queue));
            Assert.AreEqual<String>(destination.FullDestination, url);
        }
        [TestMethod]
        public void TestDestinationWithHostAndQueueSet()
        {
            String parameter = "Test";
            String urlParameter = url + "?" + parameter;
            Destination destination = new Destination(urlParameter);

            Assert.AreEqual<String>(destination.Host, url);
            Assert.AreEqual<String>(destination.Queue, parameter);
            Assert.AreEqual<String>(destination.FullDestination, urlParameter);
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInvalidAdresse1()
        {
            Destination destination = new Destination(url + "?Test?Test");
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInvalidAdresse2()
        {
            Destination destination = new Destination(url);
        }
    }
}