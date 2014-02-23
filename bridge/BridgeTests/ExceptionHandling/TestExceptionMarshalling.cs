#region Copyright
// <copyright file="TestExceptionMarshalling.cs" company="OpenEngSB">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;

namespace BridgeTests.ExceptionHandling
{
    [TestClass]
    public class TestExceptionMarshalling
    {
        private JsonMarshaller marshaller = new JsonMarshaller();

        [TestMethod]
        public void TestExceptionFromJSONToDotNetIsIJavaExceptionType()
        {
            String jsonMessage = "{\"Test1\":\"Test1\","
                                + "\"message\":\"TestException\"}";
            Assert.IsTrue(marshaller.UnmarshallObject<TestException>(jsonMessage) is IJavaException);
        }

        [TestMethod]
        public void TestExceptionFromJSONToDotNetIsTestExceptionType()
        {
            String jsonMessage = "{\"Test1\":\"Test1\","
                                + "\"message\":\"TestException\"}";
            Assert.IsTrue(marshaller.UnmarshallObject<TestException>(jsonMessage) is TestException);
        }

        [TestMethod]
        public void TestExceptionMessageFromJSONToDotNetIsCorrectlyDeserialised()
        {
            String jsonMessage = "{\"Test1\":\"Test1\","
                                + "\"message\":\"TestException\"}";
            
            IJavaException entryObject = (IJavaException) marshaller.UnmarshallObject<TestException>(jsonMessage);
            Assert.AreEqual(entryObject.Message, "TestException");
        }

        [TestMethod]
        public void TestExceptionVariableFromJSONToDotNetIsCorrectlyDeserialised()
        {
            String jsonMessage = "{\"Test1\":\"Test1\","
                                + "\"message\":\"TestException\"}";

            TestException entryObject = marshaller.UnmarshallObject<TestException>(jsonMessage);
            Assert.AreEqual(entryObject.Test1, "Test1");
        }
    }
}