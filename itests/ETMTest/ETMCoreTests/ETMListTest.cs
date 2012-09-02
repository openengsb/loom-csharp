using System.Collections.Generic;
using NUnit.Framework;
using Org.Openengsb.Loom.CSharp.Bridge.ETM.TCP;
using Org.Openengsb.Loom.CSharp.Bridge.ETM;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.ActiveMQ;
using Org.Openengsb.Loom.Csharp.Common.Bridge.Protocols.PredefinedInteractionMessage.ActiveMQ;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;
using Apache.NMS.ActiveMQ.Commands;

namespace ETMTest.ETMCoreTests
{
    [TestFixture]
    public class ETMListTest
    {
        #region Variables
        private ETMList list;
        private IList<InteractionMessage> testmessage;
        #endregion
        [TestFixtureSetUp]
        public void startup()
        {
            list = null;
            testmessage = new List<InteractionMessage>();
        }
        [Test]
        public void TestListDefaultConfigurationIsOnAllSocketPresent()
        {
            testmessage = ActiveMQConfiguration.getConfiguration();
            //KeepAlive is part of the Configuration of ActiveMQ
            IProtocol searchElement = ActiveMQConfiguration.getKeepAliveAnswer(-1).Protocol;
            InteractionMessage testresult = ActiveMQConfiguration.getKeepAliveAnswer(-1);
            list = new ETMList(testmessage);
            IProtocol result;
            for (int i = 0; i < 3; i++)
            {
                InteractionMessage interactionmessage = list.FindInteraction(searchElement.GetMessage(), i, out result);
                AreTheSame(interactionmessage, testresult);
                Assert.AreEqual(result.SocketNumber, i);
                Assert.IsTrue(searchElement.CompareTo(result) > 0);
            }
        }
        private void AreTheSame(InteractionMessage i1, InteractionMessage i2)
        {
            Assert.IsTrue(i1.SourceIPAddress.Equals(i2.SourceIPAddress));
            Assert.IsTrue(i1.DestinationIPAddress.Equals(i2.DestinationIPAddress));
            Assert.IsTrue(i1.SourcePort.Equals(i2.SourcePort));
            Assert.IsTrue(i1.Protocol.CompareTo(i2.Protocol) > 0);
        }
        [Test]
        public void TestListReturnCorrectOrderOfMessage()
        {
            testmessage = ActiveMQConfiguration.getConfiguration();
            IProtocol searchElement = new ActiveMQProtocol(new ActiveMQTextMessage("TEST1"), 2);

            InteractionMessage testresult = new InteractionMessage(null, null, searchElement, null);
            testmessage.Add(testresult);
            list = new ETMList(testmessage);
            IProtocol result;
            //First the default configuration should be found
            InteractionMessage searchresult = list.FindInteraction(searchElement.GetMessage(), 2, out result);
            AreTheSame(searchresult, testresult);
            Assert.AreEqual(searchElement.SocketNumber, 2);
            Assert.IsTrue(searchElement.CompareTo(result) > 0);
            Assert.IsTrue(((ActiveMQProtocol)searchresult.Protocol).Message is ActiveMQTextMessage);
            Assert.AreNotEqual((((ActiveMQProtocol)searchresult.Protocol).Message as ActiveMQTextMessage).Text, "TEST1");
            //Second the specific configuration should be found
            searchresult = list.FindInteraction(searchElement.GetMessage(), 2, out result);
            AreTheSame(searchresult, testresult);
            Assert.AreEqual(searchElement.SocketNumber, 2);
            Assert.IsTrue(searchElement.CompareTo(result) > 0);
            Assert.IsTrue(((ActiveMQProtocol)searchresult.Protocol).Message is ActiveMQTextMessage);
            Assert.AreEqual((((ActiveMQProtocol)searchresult.Protocol).Message as ActiveMQTextMessage).Text, "TEST1");
            //Now the first one should be found again (There is just one ActiveMQTextMessage in the DefaultConfigration and one 
            //ActiveMQTextMessage from the specific configruation => 2 ActiveMQTextMessages)
            searchresult = list.FindInteraction(searchElement.GetMessage(), 2, out result);
            AreTheSame(searchresult, testresult);
            Assert.AreEqual(searchElement.SocketNumber, 2);
            Assert.IsTrue(searchElement.CompareTo(result) > 0);
            Assert.IsTrue(((ActiveMQProtocol)searchresult.Protocol).Message is ActiveMQTextMessage);
            Assert.AreNotEqual((((ActiveMQProtocol)searchresult.Protocol).Message as ActiveMQTextMessage).Text, "TEST1");

        }
        [TestFixtureTearDown]
        public void cleanup()
        {
        }
    }
}