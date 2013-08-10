#region Copyright
// <copyright file="ExampleDomainConnector.cs" company="OpenEngSB">
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
using ExampleDomain;
using log4net;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;

namespace RuntimeTests.TestConnectorImplementation
{
    /// <summary>
    /// Example implementation of the local domain
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class ExampleDomainConnector : RegistrationFunctions, IExampleDomainSoapBinding
    {
        #region Constructors
        public ExampleDomainConnector()
        : base(LogManager.GetLogger(typeof(ExampleDomainConnector)))
        {
        }
        #endregion
        #region Public Methods
        public string doReturnMethod(string args0)
        {
            return args0;
        }

        public string doSomethingWithLogEvent(LogEvent args0)
        {
            return "DEBUG";
        }

        public string doSomethingWithMessage(string args0)
        {
            return "Hallo";
        }

        public ExampleResponseModel doSomethingWithModel(ExampleRequestModel args0)
        {
            ExampleResponseModel m = new ExampleResponseModel();
            m.result = "RESULT";
            return m;
        }

        public void getAliveState(out AliveState? @return, out bool returnSpecified)
        {
            throw new NotImplementedException();
        }

        public string getInstanceId()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}