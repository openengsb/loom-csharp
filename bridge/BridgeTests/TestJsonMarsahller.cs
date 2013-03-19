using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using OOSourceCodeDomain;
using BridgeTests.TestConnectorImplementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using OpenEngSBCore;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using BridgeTests.TestExceptionHandlers;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json;
using Newtonsoft.Json;
namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestJsonMarshaller
    {
        IMarshaller marshaller = new JsonMarshaller();
        public class Entry1
        {
            public Entry1() { }

            public string key { get; set; }
            public int value { get; set; }
        }
        /// <summary>
        /// Sell simulate the OpenEngSB generated file or the .Net Bridge
        /// </summary>
        public class ListElement
        {
            public ListElement() { }

            public List<String> stringelements { get; set; }
            public int[] intelements { get; set; }
        }
        /// <summary>
        /// Sell simulate the OpenEngSB generated file or the .Net Bridge
        /// </summary>
        public class ArrayElement
        {
            public ArrayElement() { }

            public String[] stringelements { get; set; }
            public List<int> intelements { get; set; }
        }
        public class TestClass
        {
            public TestClass() { }

            public string key { get; set; }
            public int value { get; set; }
            public Entry1[] elements { get; set; }
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInvalidSerialisation()
        {
            JsonConvert.SerializeObject(new Entry1(), new CustomJsonUnMarshaller());
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInvalidUnserialisation()
        {
            Entry1[] entry1 = new Entry1[1];
            entry1[0] = new Entry1();
            entry1[0].key = "key";
            entry1[0].value = 1;
            String msg = marshaller.MarshallObject(entry1);
            JsonConvert.DeserializeObject<Entry1[]>(msg, new CustomJsonMarshaller());
        }
        [TestMethod]
        [ExpectedException(typeof(JsonSerializationException))]
        public void TestInvalidUnserialisationArray()
        {
            TestClass[] entry1 = new TestClass[1];
            entry1[0] = new TestClass();
            entry1[0].key = "key";
            entry1[0].value = 1;
            String msg = marshaller.MarshallObject(entry1);
            //Entr1[]!=TestClass
            JsonConvert.DeserializeObject<Entry1[]>(msg, new CustomJsonUnMarshaller());
        }
        [TestMethod]
        public void TestConvertListToArray()
        {
            ListElement element = new ListElement();
            element.stringelements = new List<string>() { "Test", "Test1" };
            element.intelements = new int[] { 1, 2, 3 };
            String marshalledResult = marshaller.MarshallObject(element);
            ArrayElement result = marshaller.UnmarshallObject<ArrayElement>(marshalledResult);
            foreach (String stringelement in result.stringelements)
            {
                Assert.IsNotNull(element.stringelements.Find(e => stringelement.Equals(e)));
            }
        }

        [TestMethod]
        public void TestConvertArrayToList()
        {
            ArrayElement element = new ArrayElement();
            element.stringelements = new String[] { "Test", "Test1" };
            element.intelements = new List<int>() { 1, 2, 3 };
            String marshalledResult = marshaller.MarshallObject(element);
            ListElement result = marshaller.UnmarshallObject<ListElement>(marshalledResult);
            Assert.AreEqual<int>(element.stringelements.Length, result.stringelements.Count);
            foreach (String stringelement in element.stringelements)
            {
                Assert.IsNotNull(result.stringelements.Find(e => stringelement.Equals(e)));
            }
        }
    }
}