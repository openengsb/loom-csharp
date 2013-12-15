#region Copyright
// <copyright file="TestDestination.cs" company="OpenEngSB">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;

namespace BridgeTests.Tests
{    
    [TestClass]
    [ExcludeFromCodeCoverageAttribute]
    public class TestDestination
    {
        #region Constants
        private const String Host = "http://test.at";
        private const String Queue = "TestCase1";
        private const String Url = "localhost:8888";
        #endregion
        #region Tests
        [TestMethod]
        public void TestDestinationTypeHostAndQueueAreRecognizedCorrectly()
        {
            JmsDestination destination = new JmsDestination(Host + "?" + Queue);

            Assert.AreEqual<String>(Host, destination.Host);
            Assert.AreEqual<String>(Queue, destination.Queue);
            Assert.AreEqual<String>(Host + "?" + Queue, destination.FullDestination);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDestinationTypeWhereNoQuestionMarkIsIndicated()
        {
            JmsDestination destination = new JmsDestination(Url);
        }

        [TestMethod]
        public void TestDestinationTypeWhereNoQueueIsIndicated()
        {
            JmsDestination destination = new JmsDestination(Url + "?");

            Assert.AreEqual<String>(destination.Host, Url);
            Assert.IsTrue(String.IsNullOrEmpty(destination.Queue));
            Assert.AreEqual<String>(destination.FullDestination, Url);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestDestinationWhitInvalidParametInTheUrl()
        {
            JmsDestination destination = new JmsDestination(Url + "?Test?Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInvalidSignBetweenUrlAndParameter()
        {
            JmsDestination destination = new JmsDestination(Host + "!" + Queue);
        }
        #endregion
    }
}