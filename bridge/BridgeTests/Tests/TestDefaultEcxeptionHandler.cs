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
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestDefaultEcxeptionHandler
    {
        private const Exception nullException = null;
        private const Object nullObject = null;
        private ABridgeExceptionHandling exceptionhandler;

        [TestInitialize]
        public void Initialise()
        {
            exceptionhandler = new RetryDefaultExceptionHandler();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAbstractExceptionHandlerWithInvalidParametersThatAreAllNull()
        {
            exceptionhandler.HandleException(nullException, nullObject);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAbstractExceptionHandlerWithInvalidParametersThatAreAllNullWithStopVariableTrue()
        {
            exceptionhandler.Stop = true;

            exceptionhandler.HandleException(nullException, nullObject);
        }

        [TestMethod]
        ///The Method that will be invoked is indicated in Changed
        public void TestAbstractExceptionHandlerWithValidParametersWhereTheMethodEventReturnsTrue()
        {
            exceptionhandler.Changed += delegate(object[] obj)
            {
                return true;
            };

            Assert.IsTrue((bool)exceptionhandler.HandleException(new OpenEngSBException(), nullObject));
            Assert.IsTrue((bool)exceptionhandler.HandleException(new OpenEngSBException("Test"), nullObject));
            Assert.IsTrue((bool)exceptionhandler.HandleException(new BridgeException(), nullObject));
        }
    }
}