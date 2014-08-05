#region Copyright
// <copyright file="TestCommunicationClass.cs" company="OpenEngSB">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects;

namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute]
    public class TestCommunicationClass
    {
        #region Private Variables
        private IMarshaller marshaller = new JsonMarshaller();
        #endregion
        #region Tests
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestGenericUnMarshallExceptionWithInvalidInput()
        {
            marshaller.UnmarshallObject<RemoteMethodCall>("EXCEPTION");
        }

        [TestMethod]
        public void TestMarshallerCreatesCorrectOutput()
        {
            String result = GetJsonExampleString();
            Assert.IsTrue(result.Contains("[\"Real\"]"));
            Assert.IsTrue(result.Contains("[\"Classes\"]"));
            Assert.IsTrue(result.Contains("methodName"));
            Assert.IsTrue(result.Contains("TestCase"));
            Assert.IsTrue(result.Contains("\"callId\":\"123\""));
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestUnMarshallExceptionWithInvalidInput()
        {
            marshaller.UnmarshallObject("EXCEPTION", typeof(RemoteMethodCall));
        }


        [TestMethod]
        public void TestUnmarshallingWithGeneralTypeAndByIndicatingTypeAsParameterAreTheSame()
        {
            String result = GetJsonExampleString();
            MethodCallMessage unmarshalledResult = (MethodCallMessage)marshaller.UnmarshallObject(result, typeof(MethodCallMessage));
            MethodCallMessage unmarshalledResultType = marshaller.UnmarshallObject<MethodCallMessage>(result);

            Assert.IsTrue(unmarshalledResult.Equals(unmarshalledResultType));
        }
        #endregion
        #region Private Methods
        private string GetJsonExampleString()
        {
            BeanDescription bean = BeanDescription.CreateInstance("classname");
            RemoteMethodCall remoteMC = RemoteMethodCall.CreateInstance(
                "methodName",
                new List<Object>()
                    { 
                        "Test1"
                    },
                new Dictionary<String, String>(),
                new List<String>()
                    { 
                        "Classes"
                    },
                new List<String> 
                    { 
                    "Real" 
                    });
            MethodCallMessage message = MethodCallMessage.CreateInstance("principal", bean, remoteMC, "123", false, "TestCase");
            return marshaller.MarshallObject(message);
        }
        #endregion
    }
}