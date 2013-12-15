#region Copyright
// <copyright file="TestRemoteType.cs" company="OpenEngSB">
// Licensed to the Austrian Association for Software Tool Integration (AASTI)
// under one or more contributor license agreements. See the NOTICE file
// distributed with this work for additional information regarding copyright
// ownership. The AASTI licenses this file to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
#endregion
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;

namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute]
    public class TestRemoteType
    {
        #region Tests
        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaDoubleTypeWhenAskingTheLocalType()
        {
            ParameterInfo[] pr = typeof(BridgeTests.TestClasses.TestClassLocalType).GetMethod("hasDoubleSpecified").GetParameters();
            RemoteType lt = new RemoteType("java.lang.Double", pr);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(Double).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaDoubleTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresent()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("java.lang.DoubleExtend", pr);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(Double).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaFloatTypeWhenAskingTheLocalType()
        {
            ParameterInfo[] pr = typeof(BridgeTests.TestClasses.TestClassLocalType).GetMethod("hasFloatSpecified").GetParameters();
            RemoteType lt = new RemoteType("java.lang.Float", pr);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(float).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaFloatTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresent()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("java.lang.FloatExtend", pr);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(float).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaIntTypeWhenAskingTheLocalType()
        {
            ParameterInfo[] pr = typeof(BridgeTests.TestClasses.TestClassLocalType).GetMethod("hasIntSpecified").GetParameters();
            RemoteType lt = new RemoteType("java.lang.Integer", pr);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(int).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaIntTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresent()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("java.lang.IntegerExtend", pr);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(int).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaStringTypeWhenAskingTheLocalType()
        {
            ParameterInfo[] pr = typeof(BridgeTests.TestClasses.TestClassLocalType).GetMethod("hasStringSpecified").GetParameters();
            RemoteType lt = new RemoteType("java.lang.String", pr);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(String).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaStringTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresent()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("java.lang.String", pr);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(String).FullName);
        }

        [TestMethod]
        public void TestIfRemoteTypeReturnstheCorrectJavaStringTypeWhenAskingTheLocalTypeAndNoParameterInfoArePresentAndTheInputContainsADollar()
        {
            RemoteType lt = new RemoteType("org.openengsb$String", null);

            Assert.AreEqual<String>(lt.LocalTypeFullName, typeof(String).FullName);
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestIfRemoteTypeReturnstheExceptionIfTheInputIsInvalid()
        {
            ParameterInfo[] pr = null;
            RemoteType lt = new RemoteType("WONG", pr);
        }
        #endregion
    }
}
