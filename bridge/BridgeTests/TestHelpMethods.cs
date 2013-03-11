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
using System.Reflection;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.Remote;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;

namespace BridgeTest
{
    [TestClass]
    public class TestHelpMethods
    {
        public class Entry1
        {
            public Entry1() { }

            public string key { get; set; }
            public int value { get; set; }
        }
        public class Entry2WithoutConstructor
        {
            public string key { get; set; }
            public int value { get; set; }
        }
        [TestMethod]
        public void TestMarshallingWithOpenEngSBModel()
        {
            IMarshaller marshaller = new JsonMarshaller();
            Type type = HelpMethods.ImplementTypeDynamicly(typeof(Entry1));
            Assert.IsTrue(typeof(Type).IsInstanceOfType(typeof(OpenEngSBModel)));
            Object entryObject = Activator.CreateInstance(type);
            Assert.IsTrue(entryObject is Entry1);
            Entry1 entry = (Entry1)entryObject;
            entry.key = "test";
            entry.value = 1;

            OpenEngSBModel entryOpenEngSB = (OpenEngSBModel)entry;
            List<OpenEngSBModelEntry> elements = new List<OpenEngSBModelEntry>();
            OpenEngSBModelEntry osbEntry = new OpenEngSBModelEntry();
            osbEntry.key = "key";
            osbEntry.type = "type";
            osbEntry.value = "value";
            elements.Add(osbEntry);
            entryOpenEngSB.openEngSBModelTail = elements;

            String objString = marshaller.MarshallObject(entry);

            Assert.IsTrue(objString.ToUpper().Contains("OPENENGSBMODELTAIL"));

            OpenEngSBModel objUnmarshalled = (OpenEngSBModel)marshaller.UnmarshallObject(objString, type);
            Entry1 entryUnmarshalled = (Entry1)objUnmarshalled;

            Assert.AreEqual(elements.Count, objUnmarshalled.openEngSBModelTail.Count);
            Assert.AreEqual(objUnmarshalled.openEngSBModelTail[0].key, osbEntry.key);
            Assert.AreEqual(objUnmarshalled.openEngSBModelTail[0].type, osbEntry.type);
            Assert.AreEqual(objUnmarshalled.openEngSBModelTail[0].value, osbEntry.value);
            Assert.AreEqual(entryUnmarshalled.key, entry.key);
            Assert.AreEqual(entryUnmarshalled.value, entry.value);

        }

        [TestMethod]
        public void TestAddType()
        {
            Type type = HelpMethods.ImplementTypeDynamicly(typeof(Entry1));
            Assert.IsTrue(typeof(Type).IsInstanceOfType(typeof(OpenEngSBModel)));
            Object entryObject = Activator.CreateInstance(type);
            Assert.IsTrue(entryObject is Entry1);
            Entry1 entry = (Entry1)entryObject;
            entry.key = "test";
            entry.value = 1;

            OpenEngSBModel entryOpenEngSB = (OpenEngSBModel)entry;
            List<OpenEngSBModelEntry> elements = new List<OpenEngSBModelEntry>();
            OpenEngSBModelEntry osbEntry = new OpenEngSBModelEntry();
            osbEntry.key = "key";
            osbEntry.type = "type";
            osbEntry.value = "value";
            elements.Add(osbEntry);
            entryOpenEngSB.openEngSBModelTail = elements;
            Assert.AreEqual(elements, entryOpenEngSB.openEngSBModelTail);
            Assert.AreEqual(elements[0].key, osbEntry.key);
            Assert.AreEqual(elements[0].type, osbEntry.type);
            Assert.AreEqual(elements[0].value, osbEntry.value);
        }
        [TestMethod]
        public void TestAddTypeWithoutConstructor()
        {
            Type type = HelpMethods.ImplementTypeDynamicly(typeof(Entry2WithoutConstructor));
            Assert.IsTrue(typeof(Type).IsInstanceOfType(typeof(OpenEngSBModel)));
            Object entryObject = Activator.CreateInstance(type);
            Assert.IsTrue(entryObject is Entry2WithoutConstructor);
            Entry2WithoutConstructor entry = (Entry2WithoutConstructor)entryObject;
            entry.key = "test";
            entry.value = 1;
            OpenEngSBModel entryOpenEngSB = (OpenEngSBModel)entry;
            List<OpenEngSBModelEntry> elements = new List<OpenEngSBModelEntry>();
            OpenEngSBModelEntry osbEntry = new OpenEngSBModelEntry();
            osbEntry.key = "key";
            osbEntry.type = "type";
            osbEntry.value = "value";
            elements.Add(osbEntry);
            entryOpenEngSB.openEngSBModelTail = elements;
            Assert.AreEqual(elements, entryOpenEngSB.openEngSBModelTail);
            Assert.AreEqual(elements[0].key, osbEntry.key);
            Assert.AreEqual(elements[0].type, osbEntry.type);
            Assert.AreEqual(elements[0].value, osbEntry.value);
        }
        [TestMethod]
        public void TestMapToEntr1()
        {
            Dictionary<Object, Object> Test = new Dictionary<Object, Object>();
            Test.Add("1", 11);
            Test.Add("21", 111);
            Entry1[] result = (Entry1[])HelpMethods.ConvertMap(Test, typeof(Entry1));
            foreach (Entry1 e1 in result)
            {
                Assert.IsTrue(Test.ContainsKey(e1.key));
                Assert.AreEqual(Test[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void Entri1toMap()
        {
            Entry1[] Test = new Entry1[] { 
                new Entry1() { key = "1", value = 11 },
                new Entry1() { key = "21", value = 21 } 
            };
            IDictionary<String, int> result = HelpMethods.ConvertMap<String, int>(Test);
            foreach (Entry1 e1 in Test)
            {
                Assert.IsTrue(result.ContainsKey(e1.key));
                Assert.AreEqual(result[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void Entri1toMap_extended_Method()
        {
            Entry1[] Test = new Entry1[] { 
                new Entry1() { key = "1", value = 11 },
                new Entry1() { key = "21", value = 21 } 
            };
            IDictionary<String, int> result = Test.ConvertMap<String, int>();
            foreach (Entry1 e1 in Test)
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
            Entry1[] result = HelpMethods.ConvertMap<Entry1>(Test);
            foreach (Entry1 e1 in result)
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
            Entry1[] result = (Entry1[])Test.ConvertMap(typeof(Entry1));
            foreach (Entry1 e1 in result)
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
            Entry1[] result = Test.ConvertMap<Entry1>();
            foreach (Entry1 e1 in result)
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