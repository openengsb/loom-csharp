﻿#region Copyright
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
        private String testString = "Test";

        [TestMethod]
        public void TestExceptionFromJSONToDotNetIsIJavaExceptionType()
        {
            String jsonMessage = "{\"Test1\":\"Test1\","
                                + "\"message\":\"TestException\"}";
            Assert.IsTrue(marshaller.UnmarshallObject<TestException>(jsonMessage) is IJavaException);
        }

        [TestMethod]
        public void TestExceptionFromExceptionTestJSONMessageToDotNetIsTestExceptionType()
        {
            TestException testException = new TestException();
            testException.Test1 = testString;
            String jsonMessage = marshaller.MarshallObject(testException);
            Assert.IsTrue(marshaller.UnmarshallObject<TestException>(jsonMessage) is IJavaException);
        }

        [TestMethod]
        public void TestExceptionFromExceptionTestJSONMessageWithMessageFieldAddedToDotNetIsTestExceptionType()
        {
            TestException testException = new TestException();
            testException.Test1 = testString;
            String jsonMessage = marshaller.MarshallObject(testException);
            jsonMessage = jsonMessage.Substring(0, jsonMessage.LastIndexOf("}"));
            jsonMessage += ",\"message\":\"" + testString + "\"}";
            IJavaException exception = marshaller.UnmarshallObject<TestException>(jsonMessage) as IJavaException;
            Assert.AreEqual(exception.Message, testString);
        }
    }
}