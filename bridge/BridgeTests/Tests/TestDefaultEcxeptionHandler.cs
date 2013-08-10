#region Copyright
// <copyright file="TestDefaultEcxeptionHandler.cs" company="OpenEngSB">
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
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute]
    public class TestDefaultEcxeptionHandler
    {
        #region Constants
        private const Exception NullException = null;
        private const Object NullObject = null;
        #endregion
        #region Private Variables
        private ABridgeExceptionHandling exceptionhandler;
        #endregion
        #region Testinitialization
        [TestInitialize]
        public void Initialise()
        {
            exceptionhandler = new RetryDefaultExceptionHandler();
        }
        #endregion
        #region Tests
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAbstractExceptionHandlerWithInvalidParametersThatAreAllNull()
        {
            exceptionhandler.HandleException(NullException, NullObject);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAbstractExceptionHandlerWithInvalidParametersThatAreAllNullWithStopVariableTrue()
        {
            exceptionhandler.Stop = true;

            exceptionhandler.HandleException(NullException, NullObject);
        }

        [TestMethod]
        public void TestAbstractExceptionHandlerWithValidParametersWhereTheMethodEventReturnsTrue()
        {
            ///The Method that will be invoked is indicated in Changed
            exceptionhandler.Changed += delegate(object[] obj)
            {
                return true;
            };

            Assert.IsTrue((bool)exceptionhandler.HandleException(new OpenEngSBException(), NullObject));
            Assert.IsTrue((bool)exceptionhandler.HandleException(new OpenEngSBException("Test"), NullObject));
            Assert.IsTrue((bool)exceptionhandler.HandleException(new BridgeException(), NullObject));
        }
        #endregion
    }
}