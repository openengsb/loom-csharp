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
using System.Web.Services.Protocols;
using Implementation;
using Implementation.Exceptions;
using NUnit.Framework;

namespace BridgeTest
{
    [TestFixture]
    public class TestHelpMethods
    {
        [Test]
        public void TestreverseURL()
        {
            Assert.AreEqual(HelpMethods.reverseURL("http://example.domain.openengsb.org"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.reverseURL("http://example.domain.openengsb.org/"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.reverseURL("example.domain.openengsb.org"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.reverseURL("example.domain.openengsb.org/"), "org.openengsb.domain.example");
        }
        [Test]
        public void TestaddTrueForSpecified1()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            TestClass testClass = new TestClass();
            HelpMethods.addTrueForSpecified(elements, testClass.GetType().GetMethod("hasSpecified"));
            Assert.IsTrue(elements.Count == 2);
            Assert.IsTrue(elements[1].Equals(true));
        }
        [Test]
        public void TestaddTrueForSpecified2()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            elements.Add("test1");
            TestClass testClass = new TestClass();
            HelpMethods.addTrueForSpecified(elements, testClass.GetType().GetMethod("hasStringSpecified"));
            Assert.IsTrue(elements.Count == 2);
            Assert.IsTrue(elements[1].Equals("test1"));
        }
        [Test]
        public void TestaddTrueForSpecified3()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            elements.Add("test1");
            TestClass testClass = new TestClass();
            HelpMethods.addTrueForSpecified(elements, testClass.GetType().GetMethod("hasNoSpecified"));
            Assert.IsTrue(elements.Count == 2);
            Assert.IsTrue(elements[1].Equals("test1"));
        }
        [Test]
        public void TestaddTrueForSpecified4()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            TestClass testClass = new TestClass();
            HelpMethods.addTrueForSpecified(elements, testClass.GetType().GetMethod("hasOnlyOneField"));
            Assert.IsTrue(elements.Count == 1);
            Assert.IsTrue(elements[0].Equals("test"));
        }
        [Test]
        public void TestTypesAreEqual1()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("String");
            parameters.Add("Boolean");
            TestClass testClass = new TestClass();
            Assert.IsTrue(HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasSpecified").GetParameters()));
        }
        [Test]
        public void TestTypesAreEqual2()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("String");
            TestClass testClass = new TestClass();
            try
            {
                HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasSpecified").GetParameters());
                Assert.Fail();
            }
            catch (BridgeException)
            {
            }
        }
        [Test]
        public void TestcreateClassWithPackageName()
        {
            Assert.AreEqual(HelpMethods.createClassWithPackageName("hasStringSpecified", typeof(TestClass)), "org.openengsb.domain.example.TestClass");
        }
    }
    public class TestClass
    {
        public void hasSpecified(String test, Boolean testSpecified) { }
        [SoapDocumentMethod("urn:raiseEvent", RequestNamespace = "http://example.domain.openengsb.org", ParameterStyle = SoapParameterStyle.Wrapped)]
        public void hasStringSpecified(String test, String testSpecified) { }
        public void hasNoSpecified(String test, String test2) { }
        public void hasOnlyOneField(String test) { }
    }
}