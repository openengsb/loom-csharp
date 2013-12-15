#region Copyright
// <copyright file="TestExceptionHandler.cs" company="OpenEngSB">
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
using System.Threading;
using BridgeTests.TestConnectorImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;

namespace BridgeTests.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute]
    public class TestExceptionHandler
    {
        #region Constants
        private const String DomainName = "example";
        private const String NullString = null;
        private const String Version = "3.0.0";
        #endregion
        #region Tests
        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void TestForwardExceptionHandlerForwardsTheException()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance(Version, NullString, new ExampleDomainConnector(), new ForwardDefaultExceptionHandler());
            factory.CreateDomainService(DomainName);
        }

        [Ignore]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRetryExceptionHandlerRetiesTheMethod()
        {
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance(Version, "FooBar", new ExampleDomainConnector(), new RetryDefaultExceptionHandler());
            
            // Visual Studio unit tests does not work, when a threads are used.
            factory.StopAllConnections();
            factory.CreateDomainService(DomainName);
        }
        #endregion
    }
}