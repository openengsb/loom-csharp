#region Copyright
// <copyright file="TestCustomExceptionHandler.cs" company="OpenEngSB">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace BridgeTests.TestExceptionHandlers
{
    [ExcludeFromCodeCoverageAttribute]
    public class TestCustomExceptionHandler : ABridgeExceptionHandling
    {
        #region Private Static Variables
        /// <summary>
        /// Used for to know the test execution (test cases)
        /// </summary>
        private static int executions = 0;
        #endregion
        #region Public Methods
        /// <summary>
        /// Directly forwards the exception to user user of the .Net Bridge
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Object HandleException(Exception exception, params Object[] obj)
        {
            executions++;
            if (executions < 2)
            {
                return Invoke(obj);
            }

            return "TestCase";
        }
        #endregion
    }
}