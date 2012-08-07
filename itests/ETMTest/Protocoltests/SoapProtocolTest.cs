using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.ActiveMQ;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.Soap;

namespace ETMTest.Protocoltests
{

    [TestFixture]
    public class SoapProtocolTest
    {
        private SoapEnvelope protocol;
        [TestFixtureSetUp]
        public void StartUp()
        {
            protocol = null;
        }
        [Test]
        public void TestCorrectEnvelope()
        {
            String soap = "POST /ServiceCE/Service.svc HTTP/1.1" + Environment.NewLine +
                "Content-Type: text/xml; charset=utf-8" + Environment.NewLine +
                "SOAPAction: http://tempuri.org/IService/compile" + Environment.NewLine +
                "Host: 127.0.0.1:1866" + Environment.NewLine +
                "Content-Length: 132" + Environment.NewLine +
                "Expect: 100-continue" + Environment.NewLine +
                "Accept-Encoding: gzip, deflate" + Environment.NewLine +
                "Connection: Keep-Alive" + Environment.NewLine +
                "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope\">" + Environment.NewLine +
                    "<s:Body>" + Environment.NewLine +
                        "<HelloWorld xmlns=\"http://tempuri.org/\"/>" + Environment.NewLine +
                    "</s:Body>" + Environment.NewLine +
                "</s:Envelope>";
            protocol = new SoapEnvelope(soap);
            soap = soap.ToUpper().Replace("\n", "").Replace("\r", "");
            String result = protocol.ToString().ToUpper().Replace("\n", "").Replace("\r", "");
            Assert.AreEqual(soap, result);
        }
        [Test]
        public void TestDeserialisation()
        {
            String soap = "POST /ServiceCE/Service.svc HTTP/1.1" + Environment.NewLine +
                "Content-Type: text/xml; charset=utf-8" + Environment.NewLine +
                "SOAPAction: http://tempuri.org/IService/compile" + Environment.NewLine +
                "Host: 127.0.0.1:1866" + Environment.NewLine +
                "Content-Length: 132" + Environment.NewLine +
                "Expect: 100-continue" + Environment.NewLine +
                "Accept-Encoding: gzip, deflate" + Environment.NewLine +
                "Connection: Keep-Alive" + Environment.NewLine +
                "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope\">" + Environment.NewLine +
                    "<s:Body>" + Environment.NewLine +
                        "<HelloWorld xmlns=\"http://tempuri.org/\"/>" + Environment.NewLine +
                    "</s:Body>" + Environment.NewLine +
                "</s:Envelope>";


            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            Byte[] sopabyte = enc.GetBytes(soap);
            protocol = (SoapEnvelope)protocol.ConvertToProtocol(sopabyte);
            soap = soap.ToUpper().Replace("\n", "").Replace("\r", "");
            String result = protocol.ToString().ToUpper().Replace("\n", "").Replace("\r", "");
            Assert.AreEqual(soap, result);

        }
        [TestFixtureTearDown]
        public void Shutdwon()
        {
            protocol = null;
        }
    }
}