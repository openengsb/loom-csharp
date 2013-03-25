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
        private const String host = "http://test.at";
        private const String queue = "TestCase1";

        [TestMethod]
        public void TestDestinationTypeHostAndQueueAreRecognizedCorrectly()
        {
            Destination destination = new Destination(host + "?" + queue);

            Assert.AreEqual<String>(host, destination.Host);
            Assert.AreEqual<String>(queue, destination.Queue);
            Assert.AreEqual<String>(host + "?" + queue, destination.FullDestination);
        }

        [TestMethod]
        public void TestDestinationTypeWhereNoQueueIsIndicated()
        {
            Destination destination = new Destination(url + "?");

            Assert.AreEqual<String>(destination.Host, url);
            Assert.IsTrue(String.IsNullOrEmpty(destination.Queue));
            Assert.AreEqual<String>(destination.FullDestination, url);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInvalidSignBetweenUrlAndParameter()
        {
            Destination destination = new Destination(host + "!" + queue);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDestinationWhitInvalidParametInTheUrl()
        {
            Destination destination = new Destination(url + "?Test?Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDestinationTypeWhereNoQuestionMarkIsIndicated()
        {
            Destination destination = new Destination(url);
        }
    }
}