#region Copyright
// <copyright file="TestJsonMarshaller.cs" company="OpenEngSB">
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
using Newtonsoft.Json;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300.Remote.RemoteObjects;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.Enumeration;

namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute]
    public class TestJsonMarshaller
    {
        #region Private Variables
        private IMarshaller marshaller = new JsonMarshaller();
        #endregion
        #region Tests

        [TestMethod]
        public void TestNullValuesUnMarshalling()
        {
            BeanDescription description = new BeanDescription();
            description.ClassName = "test";
            description.Data = new Dictionary<String, string>();
            description.BinaryData = null;

            BeanDescription descriptionUnmarshalled = marshaller.UnmarshallObject<BeanDescription>(marshaller.MarshallObject(description));

            Assert.AreEqual(descriptionUnmarshalled.BinaryData, null);
        }

        [TestMethod]
        public void TestEnumValuesUnMarshalling()
        {
            ReturnType returnType = ReturnType.Void;
            ReturnType returnTypeUnmarshalled = marshaller.UnmarshallObject<ReturnType>(marshaller.MarshallObject(returnType));

            Assert.AreEqual(returnType, returnTypeUnmarshalled);
        }

        [TestMethod]
        public void TestMarshallingDictionaryThatImplementsICollection()
        {
            String key="Foo";
            String value="bar";
            BeanDescription description = new BeanDescription();
            description.ClassName = "test";
            description.Data = new Dictionary<String, string>();
            description.Data.Add(key, value);
            description.BinaryData = null;

            BeanDescription descriptionUnmarshalled = marshaller.UnmarshallObject<BeanDescription>(marshaller.MarshallObject(description));

            Assert.AreEqual(descriptionUnmarshalled.Data[key], value);
        }

        [TestMethod]
        public void TestMarshallingListThatImplementsICollection()
        {
            String value = "Foo bar";
            RemoteMethodCall description = new RemoteMethodCall();
            description.Classes = new List<String>() { value };
            RemoteMethodCall descriptionUnmarshalled = marshaller.UnmarshallObject<RemoteMethodCall>(marshaller.MarshallObject(description));

            Assert.IsTrue(descriptionUnmarshalled.Classes.Contains(value));
        }

        [TestMethod]
        public void TestIfMarshallerConvertsBetweenTheSameTypeStructureOnlyArrayInsteadOfLists()
        {
            ListElement element = new ListElement();
            element.stringelements = new List<string>()
            { 
                "Test",
                "Test1"
            };
            element.intelements = new int[] { 1, 2, 3 };
            String marshalledResult = marshaller.MarshallObject(element);
            ArrayElement result = marshaller.UnmarshallObject<ArrayElement>(marshalledResult);

            foreach (String stringelement in result.stringelements)
            {
                Assert.IsNotNull(element.stringelements.Find(e => stringelement.Equals(e)));
            }
        }

        [TestMethod]
        public void TestIfMarshallerConvertsBetweenTheSameTypeStructureOnlyListInsteadOfArray()
        {
            ArrayElement element = new ArrayElement();
            element.stringelements = new String[] { "Test", "Test1" };
            element.intelements = new List<int>()
            {
                1, 2, 3
            };

            String marshalledResult = marshaller.MarshallObject(element);
            ListElement result = marshaller.UnmarshallObject<ListElement>(marshalledResult);

            Assert.AreEqual<int>(element.stringelements.Length, result.stringelements.Count);
            foreach (String stringelement in element.stringelements)
            {
                Assert.IsNotNull(result.stringelements.Find(e => stringelement.Equals(e)));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInvalidDeserialisationWithTheCustomJsonMarshallerAsCusomMarshaller()
        {
            StringIntMapEntry[] entry1 = new StringIntMapEntry[1];
            entry1[0] = new StringIntMapEntry();
            entry1[0].key = "key";
            entry1[0].value = 1;

            String msg = marshaller.MarshallObject(entry1);

            JsonConvert.DeserializeObject<StringIntMapEntry[]>(msg, new CustomJsonMarshaller());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInvalidSerialisationWithTheCustomJsonUnMarshallerAsCusomMarshaller()
        {
            JsonConvert.SerializeObject(new StringIntMapEntry[1], new CustomJsonUnMarshaller());
        }

        [TestMethod]
        [ExpectedException(typeof(JsonSerializationException))]
        public void TestInvalidUnserialisationOfAnArrayByIndicatingTheWrongGenericTyp()
        {
            // This method fails because of Entr1[]!=TestClass
            TestClass[] entry1 = new TestClass[1];
            entry1[0] = new TestClass();
            entry1[0].key = "key";
            entry1[0].value = 1;

            String msg = marshaller.MarshallObject(entry1);

            // Entr1[]!=TestClass
            JsonConvert.DeserializeObject<StringIntMapEntry[]>(msg, new CustomJsonUnMarshaller());
        }
        #endregion
        #region Test classes
        /// <summary>
        /// Sell simulate the OpenEngSB generated file or the .Net Bridge
        /// </summary>
        public class ArrayElement
        {
            public ArrayElement()
            {
            }

            public List<int> intelements
            {
                get;
                set;
            }

            public String[] stringelements
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Sell simulate the OpenEngSB generated file or the .Net Bridge
        /// </summary>
        public class ListElement
        {
            public ListElement()
            {
            }

            public int[] intelements
            {
                get;
                set;
            }

            public List<String> stringelements
            {
                get;
                set;
            }
        }

        public class StringIntMapEntry
        {
            public StringIntMapEntry()
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

        public class TestClass
        {
            public TestClass()
            {
            }

            public StringIntMapEntry[] elements
            {
                get;
                set;
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
        #endregion
    }
}