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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;

namespace BridgeTest
{
    [TestClass]
    public class TestHelpMethods
    {
        public class entry1
        {
            public entry1() { }

            public string key { get; set; }
            public int value { get; set; }
        }
        [TestMethod]
        public void TestMapToEntr1()
        {
            Dictionary<Object, Object> Test = new Dictionary<Object, Object>();
            Test.Add("1", 11);
            Test.Add("21", 111);
            entry1[] result = (entry1[])HelpMethods.ConvertMap(Test, typeof(entry1));
            foreach (entry1 e1 in result)
            {
                Assert.IsTrue(Test.ContainsKey(e1.key));
                Assert.AreEqual(Test[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void Entri1toMap()
        {
            entry1[] Test = new entry1[] { 
                new entry1() { key = "1", value = 11 },
                new entry1() { key = "21", value = 21 } 
            };
            IDictionary<String, int> result = HelpMethods.ConvertMap<String, int>(Test);
            foreach (entry1 e1 in Test)
            {
                Assert.IsTrue(result.ContainsKey(e1.key));
                Assert.AreEqual(result[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void Entri1toMap_extended_Method()
        {
            entry1[] Test = new entry1[] { 
                new entry1() { key = "1", value = 11 },
                new entry1() { key = "21", value = 21 } 
            };
            IDictionary<String, int> result = Test.ConvertMap<String, int>();
            foreach (entry1 e1 in Test)
            {
                Assert.IsTrue(result.ContainsKey(e1.key));
                Assert.AreEqual(result[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void TestMapToEntr1_genericType()
        {
            Dictionary<Object, Object> Test = new Dictionary<Object, Object>();
            Test.Add("1", 11);
            Test.Add("21", 111);
            entry1[] result = HelpMethods.ConvertMap<entry1>(Test);
            foreach (entry1 e1 in result)
            {
                Assert.IsTrue(Test.ContainsKey(e1.key));
                Assert.AreEqual(Test[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void TestMapToEntr1_extendeMethod_with_typeParameter()
        {
            Dictionary<Object, Object> Test = new Dictionary<Object, Object>();
            Test.Add("1", 11);
            Test.Add("21", 111);
            entry1[] result = (entry1[])Test.ConvertMap(typeof(entry1));
            foreach (entry1 e1 in result)
            {
                Assert.IsTrue(Test.ContainsKey(e1.key));
                Assert.AreEqual(Test[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void TestMapToEntr1_extendeMethod_with_genericType()
        {
            Dictionary<Object, Object> Test = new Dictionary<Object, Object>();
            Test.Add("1", 11);
            Test.Add("21", 111);
            entry1[] result = Test.ConvertMap<entry1>();
            foreach (entry1 e1 in result)
            {
                Assert.IsTrue(Test.ContainsKey(e1.key));
                Assert.AreEqual(Test[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void TestreverseURL()
        {
            Assert.AreEqual(HelpMethods.ReverseURL("http://example.domain.openengsb.org"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.ReverseURL("http://example.domain.openengsb.org/"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.ReverseURL("example.domain.openengsb.org"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.ReverseURL("example.domain.openengsb.org/"), "org.openengsb.domain.example");
        }
        [TestMethod]
        public void TestaddTrueForSpecified1()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            TestClass testClass = new TestClass();
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasSpecified"));
            Assert.IsTrue(elements.Count == 2);
            Assert.IsTrue(elements[1].Equals(true));
        }
        [TestMethod]
        public void TestaddTrueForSpecified2()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            elements.Add("test1");
            TestClass testClass = new TestClass();
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasStringSpecified"));
            Assert.IsTrue(elements.Count == 2);
            Assert.IsTrue(elements[1].Equals("test1"));
        }
        [TestMethod]
        public void TestaddTrueForSpecified3()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            elements.Add("test1");
            TestClass testClass = new TestClass();
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasNoSpecified"));
            Assert.IsTrue(elements.Count == 2);
            Assert.IsTrue(elements[1].Equals("test1"));
        }
        [TestMethod]
        public void TestaddTrueForSpecified4()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            TestClass testClass = new TestClass();
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasOnlyOneField"));
            Assert.IsTrue(elements.Count == 1);
            Assert.IsTrue(elements[0].Equals("test"));
        }
        [TestMethod]
        public void TestTypesAreEqual1()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("String");
            parameters.Add("Boolean");
            TestClass testClass = new TestClass();
            Assert.IsTrue(HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasSpecified").GetParameters()));
        }
        [TestMethod]
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
        [TestMethod]
        public void TestcreateClassWithPackageName()
        {
            Assert.AreEqual(HelpMethods.CreateClassWithPackageName("hasStringSpecified", typeof(TestClass)), "org.openengsb.domain.example.TestClass");
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