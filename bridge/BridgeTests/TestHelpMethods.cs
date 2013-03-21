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
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace BridgeTests
{
    [ExcludeFromCodeCoverageAttribute()]
    [TestClass]
    public class TestHelpMethods
    {
        public class Entry1
        {
            public Entry1() { }

            public string key { get; set; }
            public int value { get; set; }
        }
        public class EntryWithEntryParameter
        {
            public EntryWithEntryParameter() { }

            public string key { get; set; }
            public Entry1[] value { get; set; }
        }
        public class EntryWithAllEntryParameter
        {
            public EntryWithAllEntryParameter() { }

            public Entry1[] key { get; set; }
            public Entry1[] value { get; set; }
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
            Object entryObject = Activator.CreateInstance(type);
            Assert.IsTrue(entryObject is Entry1);
            Assert.IsTrue(entryObject is OpenEngSBModel);
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
        public void TestAddOpenEngSBToType()
        {
            Entry1 testentry = new Entry1();
            testentry.key = "Test";
            testentry.value = 2;

            List<OpenEngSBModelEntry> elements = new List<OpenEngSBModelEntry>();
            OpenEngSBModelEntry osbEntry = new OpenEngSBModelEntry();
            osbEntry.key = "key";
            osbEntry.type = "type";
            osbEntry.value = "value";
            elements.Add(osbEntry);
            testentry=testentry.AddOpenEngSBModel<Entry1>(elements);
            OpenEngSBModel objUnmarshalled = testentry as OpenEngSBModel;
            Assert.AreEqual(elements.Count, objUnmarshalled.openEngSBModelTail.Count);
            Assert.AreEqual(objUnmarshalled.openEngSBModelTail[0].key, osbEntry.key);
            Assert.AreEqual(objUnmarshalled.openEngSBModelTail[0].type, osbEntry.type);
            Assert.AreEqual(objUnmarshalled.openEngSBModelTail[0].value, osbEntry.value);
            Assert.AreEqual(((Entry1)objUnmarshalled).key, testentry.key);
            Assert.AreEqual(((Entry1)objUnmarshalled).value, testentry.value);

        }

        [TestMethod]
        public void TestAddType()
        {
            Type type = HelpMethods.ImplementTypeDynamicly(typeof(Entry1));
            Object entryObject = Activator.CreateInstance(type);
            Assert.IsTrue(entryObject is OpenEngSBModel);
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
        public void TestAddTypeOfTypeObject()
        {
            Type type = HelpMethods.ImplementTypeDynamicly(typeof(Object));
            Object entryObject = Activator.CreateInstance(type);
            Assert.IsFalse(entryObject is OpenEngSBModel);
        }
        [TestMethod]
        public void TestAddTypeWithoutConstructor()
        {
            Type type = HelpMethods.ImplementTypeDynamicly(typeof(Entry2WithoutConstructor));
            Object entryObject = Activator.CreateInstance(type);
            Assert.IsTrue(entryObject is Entry2WithoutConstructor);
            Assert.IsTrue(entryObject is OpenEngSBModel);
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
            Entry1[] result = (Entry1[])ExtendMethods.ConvertMap(Test, typeof(Entry1));
            foreach (Entry1 e1 in result)
            {
                Assert.IsTrue(Test.ContainsKey(e1.key));
                Assert.AreEqual(Test[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void TestEntri1toMap()
        {
            Entry1[] Test = new Entry1[] { 
                new Entry1() { key = "1", value = 11 },
                new Entry1() { key = "21", value = 21 } 
            };
            IDictionary<String, int> result = ExtendMethods.ConvertMap<String, int>(Test);
            foreach (Entry1 e1 in Test)
            {
                Assert.IsTrue(result.ContainsKey(e1.key));
                Assert.AreEqual(result[e1.key], e1.value);
            }
        }
        [TestMethod]
        public void TestEntri1toMap_extended_Method()
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
        public void TestConvertDictionaryToEntry1()
        {
            IDictionary result = new Dictionary<String, int>();
            result.Add("Test", 123);
            Entry1[] arrays = (Entry1[])result.ConvertMap(typeof(Entry1[]));
        }
        [TestMethod]
        public void TestConvertXLinkConnectorViewTypesFromDiffrentDlls()
        {
            OOSourceCodeDomain.XLinkConnectorView oosxlink = new OOSourceCodeDomain.XLinkConnectorView();
            oosxlink.name = "TestCase";
            oosxlink.viewId = "TestViewId";
            OpenEngSBCore.XLinkConnectorView openEngSBCOre = oosxlink.ConvertOSBType<OpenEngSBCore.XLinkConnectorView>();
            Assert.AreEqual<String>(oosxlink.name, openEngSBCOre.name);
            Assert.AreEqual<String>(oosxlink.viewId, openEngSBCOre.viewId);
        }
        [TestMethod]
        public void TestConvertXLinkConnectorViewTypesFromDiffrentDllsAndAllValuesSet()
        {
            OOSourceCodeDomain.XLinkConnectorView oosxlink = new OOSourceCodeDomain.XLinkConnectorView();
            oosxlink.name = "TestCase";
            oosxlink.viewId = "TestViewId";
            OOSourceCodeDomain.entry1[] entry = new OOSourceCodeDomain.entry1[1];
            entry[0] = new OOSourceCodeDomain.entry1();
            entry[0].key = "key";
            entry[0].value = "value";
            oosxlink.descriptions = entry;
            OpenEngSBCore.XLinkConnectorView openEngSBCOre = oosxlink.ConvertOSBType<OpenEngSBCore.XLinkConnectorView>();
            Assert.AreEqual<String>(oosxlink.name, openEngSBCOre.name);
            Assert.AreEqual<String>(oosxlink.viewId, openEngSBCOre.viewId);
            Assert.AreEqual<String>(oosxlink.descriptions[0].key, entry[0].key);
            Assert.AreEqual<String>(oosxlink.descriptions[0].value, entry[0].value);
        }
        [TestMethod]
        public void TestConvertDictionaryToEntry1WithParameterTypeWrong()
        {
            //TODO
            IDictionary result = new Dictionary<String, String>();
            Entry1[] entry = new Entry1[] { new Entry1() { key = "Test", value = 123 } };
            result.Add("123", new JsonMarshaller().MarshallObject(entry));
            EntryWithEntryParameter[] arrays = (EntryWithEntryParameter[])result.ConvertMap(typeof(EntryWithEntryParameter[]));
            Assert.IsTrue(arrays[0].key.Equals("123"));
            Assert.IsTrue(arrays[0].value[0].key.Equals("Test"));
            result.Clear();
            result.Add(new JsonMarshaller().MarshallObject(entry), new JsonMarshaller().MarshallObject(entry));

            EntryWithAllEntryParameter[] tmpresult = (EntryWithAllEntryParameter[])result.ConvertMap(typeof(EntryWithAllEntryParameter[]));
            Assert.IsTrue(tmpresult[0].key[0].key.Equals("Test"));
            Assert.IsTrue(tmpresult[0].value[0].key.Equals("Test"));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIDictionaryConvertTo()
        {
            String test = "Error";
            test.ConvertMap<String, String>();
        }
        [TestMethod]
        public void TestMapToEntr1_genericType()
        {
            Dictionary<Object, Object> Test = new Dictionary<Object, Object>();
            Test.Add("1", 11);
            Test.Add("21", 111);
            Entry1[] result = ExtendMethods.ConvertMap<Entry1>(Test);
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConvertMapWithWrongParameters()
        {
            String noDictionary = "Empty";
            Object result = noDictionary.ConvertMap();
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
        public void TestReverseURL()
        {
            Assert.AreEqual(HelpMethods.ReverseURL("http://example.domain.openengsb.org"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.ReverseURL("http://example.domain.openengsb.org/"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.ReverseURL("example.domain.openengsb.org"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.ReverseURL("example.domain.openengsb.org/"), "org.openengsb.domain.example");
        }
        [TestMethod]
        public void TestAddTrueForSpecified1()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            TestClass testClass = new TestClass();
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasSpecified"));
            Assert.IsTrue(elements.Count == 2);
            Assert.IsTrue(elements[1].Equals(true));
        }
        [TestMethod]
        public void TestAddTrueForSpecifiedWithParameterInfo1()
        {
            List<ParameterInfo> elements = new List<ParameterInfo>();
            TestClass testClass = new TestClass();
            ParameterInfo[] tmp = testClass.GetType().GetMethod("hasSpecified").GetParameters();
            foreach (ParameterInfo pi in tmp)
            {
                elements.Add(pi);
            }
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasSpecified"));
            Assert.IsTrue(elements.Count == 1);
            Assert.IsTrue(elements[0] is object);
        }
        [TestMethod]
        public void TestAddTrueForSpecifiedWithParameterInfoNotBoolean()
        {
            List<ParameterInfo> elements = new List<ParameterInfo>();
            TestClass testClass = new TestClass();
            ParameterInfo[] tmp = testClass.GetType().GetMethod("hasNoSpecified").GetParameters();
            foreach (ParameterInfo pi in tmp)
            {
                elements.Add(pi);
            }
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasNoSpecified"));
            Assert.IsTrue(elements.Count == 2);
        }
        [TestMethod]
        public void TestAddTrueForSpecified2()
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
        public void TestAddTrueForSpecified3()
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
        public void TestAddTrueForSpecified4()
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
        [ExpectedException(typeof(BridgeException))]
        public void TestTypesAreEqual2()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("String");
            TestClass testClass = new TestClass();
            HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasSpecified").GetParameters());
        }
        [TestMethod]
        public void TestTypesAreNotEqual()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("Integer");
            parameters.Add("Bo olean");
            TestClass testClass = new TestClass();
            Assert.IsFalse(HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasSpecified").GetParameters()));
        }
        [TestMethod]
        public void TestTypesAreEqualWithMethodParameterAsObjactTypeAndNullable()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("Integer");
            parameters.Add("Boolean");
            TestClass testClass = new TestClass();
            Assert.IsTrue(HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasObjectSpecified").GetParameters()));
        }
        [TestMethod]
        public void TestTypesAreNotEqualWithMethodParameterAsObjactTypeAndNullable()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("Integer");
            parameters.Add("int");
            TestClass testClass = new TestClass();
            Assert.IsFalse(HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasObjectSpecified").GetParameters()));
        }
        [TestMethod]
        public void TestCreateClassWithPackageName()
        {
            Assert.AreEqual(HelpMethods.CreateClassWithPackageName("hasStringSpecified", typeof(TestClass)), "org.openengsb.domain.example.TestClass");
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestCreateClassWithPackageNameDoesNotExist()
        {
            HelpMethods.CreateClassWithPackageName("hasNoSpecified", typeof(TestClass));
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestCreateClassWithPackageNameWithException()
        {
            HelpMethods.CreateClassWithPackageName("DoesNotExist", typeof(TestClass));
        }
        [TestMethod]
        public void TestCreateClassArrayWithPackageName()
        {
            Assert.AreEqual(HelpMethods.CreateClassWithPackageName("OpenEngSBCore.ModelToViewsTuple[]", typeof(OpenEngSBCore.ModelToViewsTuple[])), "[Lorg.openengsb.core.api.xlink.model.ModelToViewsTuple;");
        }
        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestCreateClassArrayWithPackageNameException()
        {
            HelpMethods.CreateClassWithPackageName("BridgeTests.TestClass[]", typeof(BridgeTests.TestClass[]));
        }
        [TestMethod]
        public void TestFirstLetterToUpper()
        {
            Assert.AreEqual<String>(HelpMethods.FirstLetterToUpper("test"), "Test");
            Assert.AreEqual<String>(HelpMethods.FirstLetterToUpper("t"), "T");
            Assert.AreEqual<String>(HelpMethods.FirstLetterToUpper(""), "");
        }

    }
    [ExcludeFromCodeCoverageAttribute()]
    public class TestClass
    {
        public void hasObjectSpecified(Object test, Boolean? testSpecified) { }
        public void hasSpecified(String test, Boolean testSpecified) { }
        [SoapDocumentMethod("urn:raiseEvent", RequestNamespace = "http://example.domain.openengsb.org", ParameterStyle = SoapParameterStyle.Wrapped)]
        public void hasArraySpecified(String[] test, Boolean testSpecified) { }
        [SoapDocumentMethod("urn:raiseEvent", RequestNamespace = "http://example.domain.openengsb.org", ParameterStyle = SoapParameterStyle.Wrapped)]
        public void hasStringSpecified(String test, String testSpecified) { }
        public void hasNoSpecified(String test, String test2) { }
        public void hasOnlyOneField(String test) { }
    }
}