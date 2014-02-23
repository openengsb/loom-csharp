#region Copyright
// <copyright file="TestHelpMethods.cs" company="OpenEngSB">
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web.Services.Protocols;
using BridgeTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenEngSBCore;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;

namespace BridgeTests.Tests
{
    [ExcludeFromCodeCoverageAttribute]
    [TestClass]
    public class TestHelpMethods
    {
        #region Private Variables
        private IMarshaller marshaller;
        #endregion
        #region Testinitialzation
        [TestInitialize]
        public void Initialize()
        {
            marshaller = new JsonMarshaller();
        }
        #endregion
        #region Tests
        [TestMethod]
        public void TestAddTrueForSpecifiedFieldsWithAnObjectListWhereNoSpecifiedIsDfinedAsParameters()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            elements.Add("test1");
            ASDTestClass testClass = new ASDTestClass();
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasNoSpecified"));

            Assert.AreEqual<int>(elements.Count, 2);
            Assert.AreEqual<String>(elements[1].ToString(), "test1");
        }

        [TestMethod]
        public void TestAddTrueForSpecifiedFieldsWithAnObjectListWhereSpecifiedIsDfinedAsParameters()
        {
            ///A specified field is a field that is created from wsdl.exe to indicated if a primitiv type like boolean is set or not
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            ASDTestClass testClass = new ASDTestClass();
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasSpecified"));

            Assert.AreEqual<int>(elements.Count, 2);
            Assert.AreEqual<Boolean>((bool)elements[1], true);
        }

        [TestMethod]
        public void TestAddTrueForSpecifiedFieldsWithAnObjectListWhereStringSpecifiedIsDfinedAsParameters()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            elements.Add("test1");
            ASDTestClass testClass = new ASDTestClass();
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasStringSpecified"));

            Assert.AreEqual<int>(elements.Count, 2);
            Assert.AreEqual<String>(elements[1].ToString(), "test1");
        }

        [TestMethod]
        public void TestAddTrueForSpecifiedFieldsWithAnObjectListWhereTheMethodHasOnlyOneParameterAsParameters()
        {
            IList<Object> elements = new List<Object>();
            elements.Add("test");
            ASDTestClass testClass = new ASDTestClass();
            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasOnlyOneField"));

            Assert.AreEqual<int>(elements.Count, 1);
            Assert.AreEqual<String>(elements[0].ToString(), "test");
        }

        [TestMethod]
        public void TestAddTrueForSpecifiedFieldsWithAnObjectListWhereTheMethodParamertsAndTheListAreNotTheSame()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("Integer");
            parameters.Add("int");
            ASDTestClass testClass = new ASDTestClass();

            Assert.IsFalse(HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasObjectSpecified").GetParameters()));
        }

        [TestMethod]
        public void TestAddTrueForSpecifiedFieldsWithAnObjectListWhereTheMethodParamertsAndTheListAreTheSame()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("Integer");
            parameters.Add("Boolean");
            ASDTestClass testClass = new ASDTestClass();

            Assert.IsTrue(HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasObjectSpecified").GetParameters()));
        }

        [TestMethod]
        public void TestAddTrueForSpecifiedFieldsWithParameterInfoAsParameters()
        {
            ASDTestClass testClass = new ASDTestClass();
            ParameterInfo[] tmp = testClass.GetType().GetMethod("hasSpecified").GetParameters();
            List<ParameterInfo> elements = new List<ParameterInfo>(tmp);

            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasSpecified"));

            Assert.AreEqual<int>(elements.Count, 1);
            Assert.IsTrue(elements[0] is object);
        }

        [TestMethod]
        public void TestAddTrueForSpecifiedFieldsWithParameterInfoAsParametersWhereNoSpecifiedIsDefined()
        {
            ASDTestClass testClass = new ASDTestClass();
            ParameterInfo[] tmp = testClass.GetType().GetMethod("hasNoSpecified").GetParameters();
            List<ParameterInfo> elements = new List<ParameterInfo>(tmp);

            HelpMethods.AddTrueForSpecified(elements, testClass.GetType().GetMethod("hasNoSpecified"));
            Assert.AreEqual<int>(elements.Count, 2);
        }

        [TestMethod]
        public void TestConvertDictionaryToEntry1WithParameterTypeAreEntry1AndInJsonFormat()
        {
            // This Test case checks if an object that is not an array is not recognized as Map and converted correctly
            IDictionary result = new Dictionary<String, String>();
            Entry1 keyEntry = new Entry1 { key = "Test", value = 123 };
            Entry1 valueEntry = new Entry1 { key = "ValueTest", value = 111 };
            result.Add(marshaller.MarshallObject(keyEntry), marshaller.MarshallObject(valueEntry));

            EntryWithAllEntryParameter[] arrays = (EntryWithAllEntryParameter[])result.ConvertMap(typeof(EntryWithAllEntryParameter));

            Assert.AreEqual<String>(arrays[0].key.key, keyEntry.key);
            Assert.AreEqual<int>(arrays[0].key.value, keyEntry.value);
            Assert.AreEqual<String>(arrays[0].value.key, valueEntry.key);
            Assert.AreEqual<int>(arrays[0].value.value, valueEntry.value);
        }

        [TestMethod]
        public void TestConvertIDictionaryToMap()
        {
            IDictionary result = new Dictionary<String, int>();
            result.Add("Test", 123);
            Entry1[] arrays = (Entry1[])result.ConvertMap(typeof(Entry1[]));
        }

        [TestMethod]
        public void TestConvertingDictionaryToEntry1WhereOneElementIsSet()
        {
            Dictionary<Object, Object> test = new Dictionary<Object, Object>();
            test.Add("1", 11);
            test.Add("21", 111);

            Entry1[] result = ExtendMethods.ConvertMap<Entry1>(test);

            foreach (Entry1 e1 in result)
            {
                Assert.IsTrue(test.ContainsKey(e1.key));
                Assert.AreEqual(test[e1.key], e1.value);
            }
        }

        [TestMethod]
        public void TestConvertingDictionaryToMap()
        {
            // Map = EntryX[]
            Dictionary<Object, Object> test = new Dictionary<Object, Object>();
            test.Add("1", 11);
            test.Add("21", 111);

            Entry1[] result = (Entry1[])ExtendMethods.ConvertMap(test, typeof(Entry1));

            foreach (Entry1 e1 in result)
            {
                Assert.IsTrue(test.ContainsKey(e1.key));
                Assert.AreEqual(test[e1.key], e1.value);
            }
        }

        [TestMethod]
        public void TestConvertingMapToDictionary()
        {
            // Map = EntryX[]
            Entry1[] test = new Entry1[]
            {
                new Entry1() { key = "1", value = 11 },
                new Entry1()
                {
                    key = "21", value = 21
                }
            };

            IDictionary<String, int> result = ExtendMethods.ConvertMap<String, int>(test);

            foreach (Entry1 e1 in test)
            {
                Assert.IsTrue(result.ContainsKey(e1.key));
                Assert.AreEqual(result[e1.key], e1.value);
            }
        }

        [TestMethod]
        public void TestConvertingMapToDictionaryWithExtendedMethod()
        {
            Entry1[] test = new Entry1[]
            {
                new Entry1() { key = "1", value = 11 },
                new Entry1()
                {
                    key = "21", value = 21
                }
            };
            IDictionary<String, int> result = test.ConvertMap<String, int>();
            foreach (Entry1 e1 in test)
            {
                Assert.IsTrue(result.ContainsKey(e1.key));
                Assert.AreEqual(result[e1.key], e1.value);
            }
        }

        [TestMethod]
        public void TestConvertingWithExtendedMethodDictionaryToEntry1()
        {
            Dictionary<Object, Object> test = new Dictionary<Object, Object>();
            test.Add("1", 11);
            test.Add("21", 111);
            Entry1[] result = test.ConvertMap<Entry1>();
            foreach (Entry1 e1 in result)
            {
                Assert.IsTrue(test.ContainsKey(e1.key));
                Assert.AreEqual<Object>(test[e1.key], e1.value);
            }
        }

        [TestMethod]
        public void TestConvertingWithExtendedMethodDictionaryToEntry1WhereTheReturnTypeIsIndicatedAsType()
        {
            Dictionary<Object, Object> test = new Dictionary<Object, Object>();
            test.Add("1", 11);
            test.Add("21", 111);

            Entry1[] result = (Entry1[])test.ConvertMap(typeof(Entry1));

            foreach (Entry1 e1 in result)
            {
                Assert.IsTrue(test.ContainsKey(e1.key));
                Assert.AreEqual(test[e1.key], e1.value);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConvertMapWithWrongStringObject()
        {
            String noDictionary = "Empty";
            Object result = noDictionary.ConvertMap();
        }

        [TestMethod]
        public void TestConvertXLinkConnectorViewTypesFromDiffrentDlls()
        {
            // Convert between OOSourceCodeDomain.XLinkConnectorView and OpenEngSBCore.XLinkConnectorView
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
            OOSourceCodeDomain.string2stringMapEntry[] entry = new OOSourceCodeDomain.string2stringMapEntry[1];
            entry[0] = new OOSourceCodeDomain.string2stringMapEntry();
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
        public void TestExtendingTypeAtRunrimeAndAddOpenEngSBModelEntries()
        {
            Type type = HelpMethods.ImplementTypeDynamicly(typeof(Entry1), typeof(IOpenEngSBModel));
            Object entryObject = Activator.CreateInstance(type);
            Entry1 entry = (Entry1)entryObject;
            entry.key = "test";
            entry.value = 1;
            List<openEngSBModelEntry> elements = new List<openEngSBModelEntry>();
            openEngSBModelEntry osbEntry = new openEngSBModelEntry();
            osbEntry.key = "key";
            osbEntry.type = "type";
            osbEntry.value = "value";
            elements.Add(osbEntry);

            IOpenEngSBModel entryOpenEngSB = (IOpenEngSBModel)entry;
            entryOpenEngSB.OpenEngSBModelTail = elements;

            String objString = marshaller.MarshallObject(entry);
            IOpenEngSBModel objUnmarshalled = (IOpenEngSBModel)marshaller.UnmarshallObject(objString, type);
            Entry1 entryUnmarshalled = (Entry1)objUnmarshalled;

            Assert.IsTrue(objString.ToUpper().Contains("OPENENGSBMODELTAIL"));
            Assert.IsTrue(entryObject is Entry1);
            Assert.IsTrue(entryObject is IOpenEngSBModel);
            Assert.AreEqual(elements.Count, objUnmarshalled.OpenEngSBModelTail.Count);
            Assert.AreEqual(objUnmarshalled.OpenEngSBModelTail[0].key, osbEntry.key);
            Assert.AreEqual(objUnmarshalled.OpenEngSBModelTail[0].type, osbEntry.type);
            Assert.AreEqual(objUnmarshalled.OpenEngSBModelTail[0].value, osbEntry.value);
            Assert.AreEqual(entryUnmarshalled.key, entry.key);
            Assert.AreEqual(entryUnmarshalled.value, entry.value);
        }

        [TestMethod]
        public void TestExtendMethodThatAddsOpenEngSBModel()
        {
            Entry1 testentry = new Entry1();
            testentry.key = "Test";
            testentry.value = 2;

            List<openEngSBModelEntry> elements = new List<openEngSBModelEntry>();
            openEngSBModelEntry osbEntry = new openEngSBModelEntry();
            osbEntry.key = "key";
            osbEntry.type = "type";
            osbEntry.value = "value";
            elements.Add(osbEntry);
            testentry = testentry.AddOpenEngSBModel<Entry1>(elements);
            IOpenEngSBModel objUnmarshalled = testentry as IOpenEngSBModel;

            Assert.AreEqual(elements.Count, objUnmarshalled.OpenEngSBModelTail.Count);
            Assert.AreEqual(objUnmarshalled.OpenEngSBModelTail[0].key, osbEntry.key);
            Assert.AreEqual(objUnmarshalled.OpenEngSBModelTail[0].type, osbEntry.type);
            Assert.AreEqual(objUnmarshalled.OpenEngSBModelTail[0].value, osbEntry.value);
            Assert.AreEqual(((Entry1)objUnmarshalled).key, testentry.key);
            Assert.AreEqual(((Entry1)objUnmarshalled).value, testentry.value);
        }

        [TestMethod]
        public void TestFirstLetterToUpperWorksCorrectly()
        {
            Assert.AreEqual<String>(HelpMethods.FirstLetterToUpper("test"), "Test");
            Assert.AreEqual<String>(HelpMethods.FirstLetterToUpper("t"), "T");
            Assert.AreEqual<String>(HelpMethods.FirstLetterToUpper(String.Empty), String.Empty);
        }

        [TestMethod]
        public void TestGettingThePackagenameAndTheClassNameFromTheAnnotation()
        {
            Assert.AreEqual<String>(HelpMethods.CreateClassWithPackageName("hasStringSpecified", typeof(ASDTestClass)), "org.openengsb.domain.example.TestClass");
        }

        [TestMethod]
        public void TestGettingThePackagenameAndTheClassNameFromTheAnnotationFromAnArrayType()
        {
            Assert.AreEqual(HelpMethods.CreateClassWithPackageName("OpenEngSBCore.ModelToViewsTuple[]", typeof(OpenEngSBCore.ModelToViewsTuple[])), "[Lorg.openengsb.core.api.xlink.model.ModelToViewsTuple;");
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestGettingThePackagenameAndTheClassNameFromTheAnnotationFromAnArrayTypeWhereTheBridgeTestClassIsNotInTheAssembly()
        {
            HelpMethods.CreateClassWithPackageName("BridgeTests.TestClass[]", typeof(ASDTestClass[]));
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestGettingThePackagenameAndTheClassNameFromTheAnnotationWhereTheMethodDoesNotExist()
        {
            HelpMethods.CreateClassWithPackageName("DoesNotExist", typeof(ASDTestClass));
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestGettingThePackagenameAndTheClassNameFromTheAnnotationWhichHasNoAnnotation()
        {
            HelpMethods.CreateClassWithPackageName("hasNoSpecified", typeof(ASDTestClass));
        }

        [TestMethod]
        public void TestIfMapIsRecognizedAndIfVariablesAreDetectedCorrectlyAndCorrectConvertedFromJsonToObject()
        {
            IDictionary result = new Dictionary<String, String>();
            Entry1[] entry = new Entry1[] 
            { 
                new Entry1()
                {
                    key = "Test", value = 123
                }
            };
            result.Add("123", new JsonMarshaller().MarshallObject(entry));
            EntryWithEntryParameter[] arrays = (EntryWithEntryParameter[])result.ConvertMap(typeof(EntryWithEntryParameter));
            Assert.IsTrue(arrays[0].key.Equals("123"));
            Assert.IsTrue(arrays[0].value[0].key.Equals("Test"));
        }

        [TestMethod]
        public void TestIfUrlsAreCorrectlyReverted()
        {
            Assert.AreEqual(HelpMethods.ReverseURL("http://example.domain.openengsb.org"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.ReverseURL("http://example.domain.openengsb.org/"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.ReverseURL("example.domain.openengsb.org"), "org.openengsb.domain.example");
            Assert.AreEqual(HelpMethods.ReverseURL("example.domain.openengsb.org/"), "org.openengsb.domain.example");
        }

        [TestMethod]
        public void TestImplementOpenEngSBModelAtRuntimeIsCorrect()
        {
            Type type = HelpMethods.ImplementTypeDynamicly(typeof(Object),typeof(IOpenEngSBModel));

            Object entryObject = Activator.CreateInstance(type);

            Assert.IsFalse(entryObject is IOpenEngSBModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInvalidConvertMapWhereMapIsAStringAndNotAnEntryX()
        {
            String test = "Error";
            test.ConvertMap<String, String>();
        }

        [TestMethod]
        public void TestTypesAreEqual1()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("String");
            parameters.Add("Boolean");
            ASDTestClass testClass = new ASDTestClass();

            Assert.IsTrue(HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasSpecified").GetParameters()));
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestTypesAreEqual2()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("String");
            ASDTestClass testClass = new ASDTestClass();

            HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasSpecified").GetParameters());
        }

        [TestMethod]
        public void TestTypesAreNotEqual()
        {
            IList<string> parameters = new List<String>();
            parameters.Add("Integer");
            parameters.Add("Bo olean");
            ASDTestClass testClass = new ASDTestClass();

            Assert.IsFalse(HelpMethods.TypesAreEqual(parameters, testClass.GetType().GetMethod("hasSpecified").GetParameters()));
        }
        #endregion
        #region Test Classes
        public class Entry1
        {
            public Entry1()
            {
            }

            public string key
            {
                get;
                set;
            }

            public int value
            {
                get;
                set;
            }
        }

        public class EntryWithAllEntryParameter
        {
            public EntryWithAllEntryParameter()
            {
            }

            public Entry1 key
            {
                get;
                set;
            }

            public Entry1 value
            {
                get;
                set;
            }
        }

        public class EntryWithEntryParameter
        {
            public EntryWithEntryParameter()
            {
            }

            public string key
            {
                get;
                set;
            }

            public Entry1[] value
            {
                get;
                set;
            }
        }
        #endregion
    }
}