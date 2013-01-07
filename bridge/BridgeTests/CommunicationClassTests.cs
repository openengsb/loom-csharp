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
namespace BridgeTest
{
    [TestClass]
    public class CommunicationClassTests
    {
        [TestMethod]
        public void DestinationTestNoException()
        {
            String host = "http://test.at";
            String queue = "TestCase1";
            Destination destination = new Destination(host + "?" + queue);
            Assert.AreEqual(host, destination.Host);
            Assert.AreEqual(queue, destination.Queue);
            Assert.AreEqual(host + "?" + queue, destination.FullDestination);
        }
        [TestMethod]
        public void DestinationTestException()
        {
            String host = "http://test.at";
            String queue = "TestCase1";
            try
            {
                Destination destination = new Destination(host + "!" + queue);
                Assert.Fail();
            }
            catch (ApplicationException) { }
        }
        [TestMethod]
        public void MarshallAndUnMarshallNoExceptionTest()
        {
            IMarshaller marshaller = new JsonMarshaller();
            BeanDescription bean = BeanDescription.createInstance("classname");
            RemoteMethodCall remoteMC = RemoteMethodCall.CreateInstance("methodName", new List<Object>() { "Test1" }, new Dictionary<String, String>(), new List<String>() { "Classes" }, new List<String> { "Real" });
            MethodCallMessage message = MethodCallMessage.createInstance("principal", bean, remoteMC, "123", false, "TestCase");
            String result = marshaller.MarshallObject(message);
            MethodCallMessage unmarshalledResult = (MethodCallMessage)marshaller.UnmarshallObject(result, typeof(MethodCallMessage));
            MethodCallMessage unmarshalledResultType = marshaller.UnmarshallObject<MethodCallMessage>(result);
            Assert.AreNotEqual(unmarshalledResult, unmarshalledResultType);
            Assert.IsTrue(result.Contains("[\"Real\"]"));
            Assert.IsTrue(result.Contains("[\"Classes\"]"));
            Assert.IsTrue(result.Contains("methodName"));
            Assert.IsTrue(result.Contains("TestCase"));
            Assert.IsTrue(result.Contains("\"callId\":\"123\""));
        }
        [TestMethod]
        public void UnMarshallExceptionTest()
        {
            IMarshaller marshaller = new JsonMarshaller();
            try
            {
                marshaller.UnmarshallObject<RemoteMethodCall>("EXCEPTION");
                Assert.Fail();
            }
            catch (BridgeException) { }
            try
            {
                marshaller.UnmarshallObject("EXCEPTION", typeof(RemoteMethodCall));
                Assert.Fail();
            }
            catch (BridgeException) { }
        }
    }
}