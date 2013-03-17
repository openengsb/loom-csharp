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
namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestDefaultEcxeptionHandler
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAbstractExceptionHandler()
        {
            ABridgeExceptionHandling exceptionhandler = new RetryDefaultExceptionHandler();
            exceptionhandler.HandleException(null, null);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAbstractExceptionHandlerWithStop()
        {
            ABridgeExceptionHandling exceptionhandler = new RetryDefaultExceptionHandler();
            exceptionhandler.Stop = true;
            exceptionhandler.HandleException(null, null);
        }
        [TestMethod]
        public void TestAbstractExceptionHandlerWithInvokeMethod()
        {
            ABridgeExceptionHandling exceptionhandler = new RetryDefaultExceptionHandler();
            exceptionhandler.Changed += delegate(object[] obj)
            {
                return true;
            };
            Assert.IsTrue((bool)exceptionhandler.HandleException(new OpenEngSBException(), null));
            Assert.IsTrue((bool)exceptionhandler.HandleException(new OpenEngSBException("Test"), null));
            Assert.IsTrue((bool)exceptionhandler.HandleException(new BridgeException(), null));
        }
    }
}