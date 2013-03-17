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
        [TestMethod]
        public void TestDestinationWithoutQueue()
        {
            Destination destination = new Destination("localhost:8888?");
            Assert.AreEqual<String>(destination.Host, "localhost:8888");
            Assert.IsTrue(String.IsNullOrEmpty(destination.Queue));
            Assert.AreEqual<String>(destination.FullDestination, "localhost:8888");
        }
        [TestMethod]
        public void TestDestinationWithHostAndQueueSet()
        {
            Destination destination = new Destination("localhost:8888?Test");
            Assert.AreEqual<String>(destination.Host, "localhost:8888");
            Assert.AreEqual<String>(destination.Queue, "Test");
            Assert.AreEqual<String>(destination.FullDestination, "localhost:8888?Test");
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInvalidAdresse1()
        {
            Destination destination = new Destination("localhost:8888?Test?Test");
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInvalidAdresse2()
        {
            Destination destination = new Destination("localhost:8888");
        }
    }
}