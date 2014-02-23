#region Copyright
// <copyright file="TestDomainFactoryProvider.cs" company="OpenEngSB">
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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;
using Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300;

namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute]
    public class TestDomainFactoryProvider
    {
        #region Constants
        private const ABridgeExceptionHandling NullExceptionHandler = null;
        private const String NullString = null;
        private const String Password = "password";
        private const String Username = "admin";
        private const String Version240 = "2.4.0";
        private const String Version300 = "3.0.0";
        private const String VersionInvalid = "1.4.0";
        private const String ContextId = "Test";
        private ABridgeExceptionHandling forwardDefaultExceptionHandler = new ForwardDefaultExceptionHandler();
        #endregion
        #region Public Methods
        [ExpectedException(typeof(BridgeException))]
        [TestMethod]
        public void TestFactoryReturnsConnector240OpenEngSBVersion()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version240, NullString, NullString);

            Assert.IsNotNull(factory);
        }

        [ExpectedException(typeof(BridgeException))]
        [TestMethod]
        public void TestFactoryReturnsConnector240OpenEngSBVersionWithExceptionHandlerParameter()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version240, NullString, NullString, NullExceptionHandler);

            Assert.IsNotNull(factory);
        }

        [ExpectedException(typeof(BridgeException))]
        [TestMethod]
        public void TestFactoryReturnsConnector240OpenEngSBVersionWithUsernameAndPassword()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version240, NullString, NullString, Username, Password);

            Assert.IsNotNull(factory);
        }

        [ExpectedException(typeof(BridgeException))]
        [TestMethod]
        public void TestFactoryReturnsConnector240OpenEngSBVersionWithUsernamePassword()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version240, NullString, NullString, NullExceptionHandler, Username, Password);

            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void TestFactoryReturnsConnector300OpenEngSBVersion()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version300, NullString, NullString);

            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void TestFactoryReturnsConnector300OpenEngSBVersionWithExceptionHandlerParameter()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version300, NullString, NullString, forwardDefaultExceptionHandler);

            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void TestFactoryReturnsConnector300OpenEngSBVersionWithUsernameAndPassword()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version300, NullString, NullString, Username, Password);

            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void TestFactoryReturnsConnector300OpenEngSBVersionWithUsernamePassword()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version300, NullString, NullString, forwardDefaultExceptionHandler, Username, Password);

            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryReturnsNullWithInvalidVersionNumberAsParameter()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(VersionInvalid, NullString, NullString);
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryReturnsNullWithInvalidVersionNumberAsParameterAndExceptionHandlerAsParameter()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(VersionInvalid, NullString, NullString, NullExceptionHandler);
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryReturnsNullWithInvalidVersionNumberAsParameterAndExceptionHandlerUsernamPassword()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(VersionInvalid, NullString, NullString, NullExceptionHandler, Username, Password);
        }

        [TestMethod]
        [ExpectedException(typeof(BridgeException))]
        public void TestFactoryReturnsNullWithInvalidVersionNumberAsParameterAndUsernamPassword()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(VersionInvalid, NullString, NullString, Username, Password);
        }

        [TestMethod]
        public void TestFactoryReturnWithVersionDomainNameserviceAndContextId()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version300, NullString, NullString, ContextId);
        }

        [TestMethod]
        public void TestFactoryReturnWithVersionDomainNameserviceExceptionHandlerAndContextId()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version300, NullString, NullString, forwardDefaultExceptionHandler, ContextId);
        }

        [TestMethod]
        public void TestFactoryReturnWithVersionDomainNameserviceExceptionHandlerUserNamePasswordAndContextId()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version300, NullString, NullString, NullExceptionHandler, Username, Password, ContextId);
        }

        [TestMethod]
        public void TestFactoryReturnWithVersionDomainNameserviceUserNamePasswordAndContextId()
        {
            OpenEngSB300.SetSupport();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance<String>(Version300, NullString, NullString, Username, Password, ContextId);
        }
        #endregion
    }
}