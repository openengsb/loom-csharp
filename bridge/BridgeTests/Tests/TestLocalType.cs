using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using OpenEngSBCore;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using LocalTypeTestClass;
using System.Diagnostics.CodeAnalysis;

namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestLocalType
    {
        [TestMethod]
        public void TestIfRemoteStringIsInJavaValid()
        {
            LocalType lt = new LocalType(typeof(String));

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "java.lang.String");
        }

        [TestMethod]
        public void TestIfRemoteIntIsInJavaValid()
        {
            LocalType lt = new LocalType(typeof(int));

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "java.lang.Integer");
        }

        [TestMethod]
        public void TestIfRemoteDoubleIsInJavaValid()
        {
            LocalType lt = new LocalType(typeof(Double));

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "java.lang.Double");
        }

        [TestMethod]
        public void TestIfRemoteFloatIsInJavaValid()
        {
            LocalType lt = new LocalType(typeof(float));

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "java.lang.Float");
        }

        [TestMethod]
        public void TestIfRemoteAliveStatIsInJavaValid()
        {
            LocalType lt = new LocalType(typeof(AliveState));

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "org.openengsb.core.api.AliveState");
        }

        [TestMethod]
        public void TestIfRemoteDictionaryIsInJavaValid()
        {
            LocalType lt = new LocalType(typeof(string2stringMapEntry));

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "java.util.Map");
        }

        [TestMethod]
        public void TestIfRemoteFromAnnotiationInAMethodIsInJavaValid()
        {
            LocalType lt = new LocalType(typeof(TestClassLocalType));

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "org.test.domain.example.TestClassLocalType");
        }
    }
}

namespace LocalTypeTestClass
{
    [XmlTypeAttribute(Namespace = "http://example.domain.test.org")]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestClassLocalType
    {
        [SoapDocumentMethod("urn:raiseEvent", RequestNamespace = "http://example.domain.openengsb.org", ParameterStyle = SoapParameterStyle.Wrapped)]
        public void hasArraySpecified(String[] test, Boolean testSpecified) { }
        [SoapDocumentMethod("urn:raiseEvent", RequestNamespace = "http://example.domain.openengsb.org", ParameterStyle = SoapParameterStyle.Wrapped)]
        public void hasStringSpecified(String test, String testSpecified) { }
    }
}