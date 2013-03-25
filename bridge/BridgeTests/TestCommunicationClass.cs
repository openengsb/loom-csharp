/***
 * Licensed to the Austrian Association for Software Tool Integration (AASTI)
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. The AASTI licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and

 * limitations under the License.
 ***/
using System;
using System.Collections.Generic;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote.RemoteObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestCommunicationClass
    {
        private IMarshaller marshaller = new JsonMarshaller();
        [TestMethod]
        public void TestUnmarshallingWithGeneralTypeAndByIndicatingTypeAsParameterAreTheSame()
        {
            String result = getJsonExampleString();
            MethodCallMessage unmarshalledResult = (MethodCallMessage)marshaller.UnmarshallObject(result, typeof(MethodCallMessage));
            MethodCallMessage unmarshalledResultType = marshaller.UnmarshallObject<MethodCallMessage>(result);

            Assert.AreEqual<Boolean>(unmarshalledResult.answer,unmarshalledResultType.answer);
            Assert.AreEqual<String>(unmarshalledResult.callId, unmarshalledResultType.callId);
            Assert.AreEqual<String>(unmarshalledResult.destination, unmarshalledResultType.destination);
            Assert.AreEqual(unmarshalledResult.methodCall.args.Count, unmarshalledResultType.methodCall.args.Count);
            Assert.AreEqual(unmarshalledResult.methodCall.classes.Count, unmarshalledResultType.methodCall.classes.Count);
            Assert.AreEqual(unmarshalledResult.methodCall.metaData.Count, unmarshalledResultType.methodCall.metaData.Count);
            Assert.AreEqual(unmarshalledResult.methodCall.methodName, unmarshalledResultType.methodCall.methodName);
            Assert.AreEqual(unmarshalledResult.methodCall.realClassImplementation.Count,
                unmarshalledResultType.methodCall.realClassImplementation.Count);
            Assert.AreEqual(unmarshalledResult.credentials.binaryData.Count, unmarshalledResultType.credentials.binaryData.Count);
            Assert.AreEqual(unmarshalledResult.credentials.className, unmarshalledResultType.credentials.className);
            Assert.AreEqual(unmarshalledResult.credentials.data.Count, unmarshalledResultType.credentials.data.Count);
            Assert.AreEqual<String>(unmarshalledResult.principal, unmarshalledResultType.principal);
            Assert.AreEqual<long>(unmarshalledResult.timestamp, unmarshalledResultType.timestamp);
        }
        [TestMethod]
        public void TestMarshallerCreatesCorrectOutput()
        {
            String result=getJsonExampleString();
            Assert.IsTrue(result.Contains("[\"Real\"]"));
            Assert.IsTrue(result.Contains("[\"Classes\"]"));
            Assert.IsTrue(result.Contains("methodName"));
            Assert.IsTrue(result.Contains("TestCase"));
            Assert.IsTrue(result.Contains("\"callId\":\"123\""));
        }
        private string getJsonExampleString()
        {
            BeanDescription bean = BeanDescription.createInstance("classname");
            RemoteMethodCall remoteMC = RemoteMethodCall.CreateInstance("methodName", new List<Object>() { "Test1" }, new Dictionary<String, String>(), new List<String>() { "Classes" }, new List<String> { "Real" });
            MethodCallMessage message = MethodCallMessage.createInstance("principal", bean, remoteMC, "123", false, "TestCase");
            return marshaller.MarshallObject(message);
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestGenericUnMarshallExceptionWithInvalidInput()
        {
            marshaller.UnmarshallObject<RemoteMethodCall>("EXCEPTION");
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestUnMarshallExceptionWithInvalidInput()
        {
            marshaller.UnmarshallObject("EXCEPTION", typeof(RemoteMethodCall));
        }
    }
}