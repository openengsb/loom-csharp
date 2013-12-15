#region Copyright
// <copyright file="TestRuntimeJmsPort.cs" company="OpenEngSB">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms;
using RuntimeTests.TestExceptionHandlers;

namespace RuntimeTests.RuntimeTests
{
    // CHECK
    public class TestRuntimeJmsPort : OSBRunTimeTestParent
    {
        #region Constants
        private const String ConnectorId = "TestCase";
        private const String TcpUrlOpenEngSB = "tcp://localhost.:6549?";
        #endregion
        #region Private Variables
        private Guid tmpGuid;
        #endregion
        #region Public Methods
        public override void CleanUp()
        {
        }

        public override void Init()
        {
            TestCustomExceptionHandler.Executions = 0;
            tmpGuid = Guid.NewGuid();
        }

        public void TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithIcomingPort()
        {
            string destination = TcpUrlOpenEngSB + tmpGuid.ToString();

            IIncomingPort inPort = new JmsIncomingPort(destination, new TestCustomExceptionHandler(), ConnectorId);
            inPort.Close();

            Assert.AreEqual<String>(inPort.Receive(), "TestCase");
        }

        public void TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithOutgoingPort()
        {
            string destination = TcpUrlOpenEngSB + tmpGuid.ToString();

            IOutgoingPort outPort = new JmsOutgoingPort(destination, new TestCustomExceptionHandler(), ConnectorId);
            outPort.Close();
            outPort.Send("Error");

            Assert.AreEqual<int>(TestCustomExceptionHandler.Executions, 2);
        }

        public void TestJmsOutgoingPortWithQueueNameExceptionHandler()
        {
            Guid tmpGuid = Guid.NewGuid();
            string destination = TcpUrlOpenEngSB + tmpGuid.ToString();

            IOutgoingPort outPort = new JmsOutgoingPort(destination, new TestCustomExceptionHandler(), ConnectorId);
            outPort.Close();
            outPort.Send("Error", "NotExist");

            Assert.AreEqual<int>(TestCustomExceptionHandler.Executions, 2);
        }

        public void TestJmsPortWithWrongUrlParameter()
        {
            string destination = "Wrong?" + tmpGuid.ToString();

            // The exceptHandler returns null and so the is connector is not stored
            try
            {
                JmsPort outPort = new JmsIncomingPort(destination, new TestCustomExceptionHandler(), ConnectorId);
                Assert.Fail();
            }
            catch
            {
            }
        }
        #endregion
    }
}