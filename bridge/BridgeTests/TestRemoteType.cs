using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;
using OpenEngSBCore;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using LocalTypeTestClass;
using System.Reflection;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestRemoteRype
    {
        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaStringTypeWhenAskingTheLocalType()
        {
            ParameterInfo[] pr = typeof(RemoteTypeTestClass.TestClassLocalType).GetMethod("hasStringSpecified").GetParameters();
            RemoteType lt = new RemoteType("java.lang.String", pr);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(String).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaIntTypeWhenAskingTheLocalType()
        {
            ParameterInfo[] pr = typeof(RemoteTypeTestClass.TestClassLocalType).GetMethod("hasIntSpecified").GetParameters();
            RemoteType lt = new RemoteType("java.lang.Integer", pr);
            
            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(int).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaDoubleTypeWhenAskingTheLocalType()
        {
            ParameterInfo[] pr = typeof(RemoteTypeTestClass.TestClassLocalType).GetMethod("hasDoubleSpecified").GetParameters();
            RemoteType lt = new RemoteType("java.lang.Double", pr);
            
            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(Double).FullName);
        }
        
        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaFloatTypeWhenAskingTheLocalType()
        {
            ParameterInfo[] pr = typeof(RemoteTypeTestClass.TestClassLocalType).GetMethod("hasFloatSpecified").GetParameters();
            RemoteType lt = new RemoteType("java.lang.Float", pr);
            
            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(float).FullName);
        }
        
        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaStringTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresent()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("java.lang.String", pr);
            
            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(String).FullName);
        }
        
        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaIntTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresent()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("java.lang.IntegerExtend", pr);
            
            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(int).FullName);
        }
        
        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaDoubleTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresent()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("java.lang.DoubleExtend", pr);
            
            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(Double).FullName);
        }
        
        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaFloatTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresent()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("java.lang.FloatExtend", pr);
            
            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(float).FullName);
        }
        
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestIfRemoteTypeReturnstheExceptionIfTheInputIsInvalid()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("WONG", pr);
        }
        
        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaStringTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresentAndTheInputContainsADollar()
        {
            RemoteType lt = new RemoteType("org.openengsb$String", null);
            
            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(String).FullName);
        }
    }
}

namespace RemoteTypeTestClass
{
    [XmlTypeAttribute(Namespace = "http://example.domain.test.org")]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestClassLocalType
    {
        public void hasArraySpecified(String[] test, Boolean testSpecified) { }
        public void hasStringSpecified(String test, String testSpecified) { }
        public void hasIntSpecified(int test, Boolean testSpecified) { }
        public void hasFloatSpecified(float test, String testSpecified) { }
        public void hasDoubleSpecified(Double test, Boolean testSpecified) { }
    }
}