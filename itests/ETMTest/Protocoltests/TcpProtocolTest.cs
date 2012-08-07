using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.Tcp;

namespace ETMTest.Protocoltests
{
    [TestFixture]
    public class TcpProtocolTest
    {
        private TCPProtocol protocol;
        [TestFixtureSetUp]
        public void StartUp()
        {
            protocol = null;
        }
        [Test]
        public void TestEquals()
        {
            Byte[] b1 = new Byte[] { 1, 2, 3, 1, 2, 3, 1, 2, 3 };
            Byte[] b2 = new Byte[] { 1, 2, 3, 1, 2, 3, 1, 2, 3, 0, 0, 0, 0, 0 };
            protocol = new TCPProtocol(b1,-1);
            Assert.IsTrue(protocol.CompareTo(protocol.ConvertToProtocol(b2)) > 0);
        }
 
        [TestFixtureTearDown]
        public void Shutdwon()
        {
            protocol = null;
        }
    }
}