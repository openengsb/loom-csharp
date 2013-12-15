#region Copyright
// <copyright file="TestLocalType.cs" company="OpenEngSB">
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
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using BridgeTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;

namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute]
    public class TestLocalType
    {
        #region Tests
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
        public void TestIfRemoteFromAnnotiationInAMethodIsInJavaValid()
        {
            Type testClassType = typeof(TestClassLocalTypeWithLessMethods);
            LocalType lt = new LocalType(testClassType);

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "org.test.domain.example." + testClassType.Name);
        }

        [TestMethod]
        public void TestIfRemoteIntIsInJavaValid()
        {
            LocalType lt = new LocalType(typeof(int));

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "java.lang.Integer");
        }

        [TestMethod]
        public void TestIfRemoteStringIsInJavaValid()
        {
            LocalType lt = new LocalType(typeof(String));

            Assert.AreEqual<String>(lt.RemoteTypeFullName, "java.lang.String");
        }
        #endregion
    }
}