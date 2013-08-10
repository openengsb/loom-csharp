#region Copyright
// <copyright file="Program.cs" company="OpenEngSB">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuntimeTests;
using RuntimeTests.RuntimeTests;

namespace RuntimeTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OSBRunTimeTestParent.OpenOSB();
            try
            {
                TestRuntimeJmsPort jms = new TestRuntimeJmsPort();
                TestRuntimeNetBridge net = new TestRuntimeNetBridge();
                TestRuntimeNetBridgeUserNameAndPassword bridge1 = new TestRuntimeNetBridgeUserNameAndPassword();
                TestRuntimeNetBridgeWrongUsernameAndPassword bridge2 = new TestRuntimeNetBridgeWrongUsernameAndPassword();
                TestRuntimeXLinkConnection xlink = new TestRuntimeXLinkConnection();

                jms.Init();
                net.Init();
                bridge1.Init();
                bridge2.Init();
                xlink.Init();

                jms.TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithIcomingPort();
                jms.TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithOutgoingPort();
                jms.TestJmsOutgoingPortWithQueueNameExceptionHandler();
                jms.TestJmsPortWithWrongUrlParameter();
                jms.CleanUp();

                net.TestConnectToXlinkWithAConnectorThatIsNotregisteredConnectorAndAnalyseTheException();
                net.TestConnectToXlinkWithNoncreatedAndNonregisteredConnectorAndCheckTheCorrectException();
                net.TestCreateDeleteConnectorAndNoRegistrationWorksCorrectly();
                net.TestCreateDeleteConnectorWithoutRegistrationWithUsernameAndPasswordWorksCorrectly();
                net.TestCreateMoreConnectorAndStopAllWorksCorrectly();
                net.TestCreateRegisterCloseRegisterWorksCorrectly();
                net.TestCreateRegisterConnectorAndGetEventHandlerWithInvalitConnectorId();
                net.TestCreateRegisterConnectXlinkDisconnectXlinkUnregisterDeleteConnectorOnADomainThatDoesNotSupportXlink();
                net.TestCreateRegisterUnregisterDeleteConnectorWorksCorrectly();
                net.TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectly();
                net.TestCreateRegisterWithEventHandlerInvokedUnregisterDeleteWorksCorrectly();
                net.CleanUp();

                bridge1.TestCreateDeleteConnectorAndNoRegistrationWorksCorrectlyWithUsernameAndPassword();
                bridge1.TestCreateDeleteConnectorWithoutRegistrationWithUsernameAndPasswordWorksCorrectlyUsernameAndPassword();
                bridge1.TestCreateRegisterEventHandlerUnregisterDeleteWorksCorrectlyWithUsernameAndPassword();
                bridge1.TestCreateRegisterUnregisterDeleteConnectorWorksCorrectlyWithUsernameAndPassword();
                bridge1.TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectlyWithUsernameAndPassword();
                bridge1.CleanUp();

                bridge2.TestCreateConnectorWithWrongUsernameAndPassword();
                bridge2.TestRegisterConnectorWithoutCreateAndWithWrongUsernameAndPassword();
                bridge2.CleanUp();

                xlink.TestCreateRegisterConnecttoxlinkDisconnectfromXlinkUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectly();
                xlink.CleanUp();
            }
            finally
            {
                OSBRunTimeTestParent.CloseOSB();
            }
        }
    }
}