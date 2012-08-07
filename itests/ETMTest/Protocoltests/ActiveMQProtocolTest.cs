using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.ActiveMQ;
using NUnit.Framework;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.ActiveMQ.OpenWire;
using AcitveMQProtocol.ActiveMQConfiguration;

namespace ETMTest.Protocoltests
{
    [TestFixture]
    public class ActiveMQProtocolTest
    {
        private ActiveMQProtocol protocol;
        [TestFixtureSetUp]
        public void StartUp()
        {
            protocol = null;
        }
        [Test]
        public void TestActiveMQTextMessage()
        {
            String stringMessage="Test";
            ActiveMQTextMessage message = new ActiveMQTextMessage(stringMessage);
            protocol = new ActiveMQProtocol(message, 1);
            Byte[] byteMessage=protocol.GetMessage();
            Command receivedMessage = ((ActiveMQProtocol)protocol.ConvertToProtocol(byteMessage)).Message;
            Assert.IsTrue(receivedMessage is ActiveMQTextMessage);
            Assert.AreEqual(((ActiveMQTextMessage)receivedMessage).Text, stringMessage);
        }
        [Test]
        public void TestWireFormat()
        {
            WireFormatInfo wire = new WireFormatInfo();
            wire.CommandId = 1;
            wire.Version = 6;
            wire.MaxInactivityDurationInitialDelay = 2000;
            wire.CacheSize = 10;
            wire.TcpNoDelayEnabled = true;
            wire.SizePrefixDisabled = true;
            wire.CacheEnabled = true;
            wire.MaxInactivityDuration = 60000;
            wire.StackTraceEnabled = true;
            wire.TightEncodingEnabled = true;

            WireFormatInfo receivedWireFormat = new WireFormatInfo();
            receivedWireFormat.CommandId = 1;
            receivedWireFormat.Version = 6;
            receivedWireFormat.MaxInactivityDurationInitialDelay = 10000;
            receivedWireFormat.CacheSize = 0;
            receivedWireFormat.TcpNoDelayEnabled = false;
            receivedWireFormat.SizePrefixDisabled = false;
            receivedWireFormat.CacheEnabled = false;
            receivedWireFormat.MaxInactivityDuration = 30000;
            receivedWireFormat.StackTraceEnabled = false;
            receivedWireFormat.TightEncodingEnabled = false;
            protocol = new ActiveMQProtocol(wire, -1); 
            new ActiveMQProtocol(receivedWireFormat, -1);

            OpenWireFormat wireToCheck = ActiveMQProtocol.format;

            Assert.AreEqual(wireToCheck.CacheEnabled, receivedWireFormat.CacheEnabled);
            Assert.AreEqual(wireToCheck.CacheSize, receivedWireFormat.CacheSize);
            Assert.AreEqual(wireToCheck.MaxInactivityDuration, receivedWireFormat.MaxInactivityDuration);
            Assert.AreEqual(wireToCheck.MaxInactivityDurationInitialDelay, receivedWireFormat.MaxInactivityDurationInitialDelay);
            Assert.AreEqual(wireToCheck.StackTraceEnabled, receivedWireFormat.StackTraceEnabled);
            Assert.AreEqual(wireToCheck.TcpNoDelayEnabled, receivedWireFormat.TcpNoDelayEnabled);
            Assert.AreEqual(wireToCheck.TightEncodingEnabled, receivedWireFormat.TightEncodingEnabled);
        }
        [Test]
        public void TestSerialisationIsTheSame()
        {
            String stringMessage = "Test";
            ActiveMQTextMessage message = new ActiveMQTextMessage(stringMessage);
            ActiveMQTextMessage message2 = new ActiveMQTextMessage(stringMessage);
            protocol = new ActiveMQProtocol(message, 1);
            Byte[] byteMessage = protocol.GetMessage();

            protocol = new ActiveMQProtocol(message, 1);        
            Assert.AreEqual(byteMessage, protocol.GetMessage());
        }
        [Test]
        public void TestSerialisationAreDiffrent()
        {
            String stringMessage = "Test";
            ActiveMQProtocol.format.TightEncodingEnabled = true;
            ActiveMQProtocol.format.SizePrefixDisabled = true;

            ActiveMQTextMessage message = new ActiveMQTextMessage(stringMessage);
            protocol = new ActiveMQProtocol(message, 1);
            Byte[] byteMessage = protocol.GetMessage();

            WireFormatInfo receivedWireFormat = ActiveMQConfiguration.getWireFormatInfo();
            receivedWireFormat.TightEncodingEnabled = false;
            new ActiveMQProtocol(receivedWireFormat, -1);

            Assert.AreNotEqual(byteMessage, protocol.GetMessage());
        }
        [TestFixtureTearDown]
        public void Shutdwon()
        {
            protocol = null;
        }
    }
}
