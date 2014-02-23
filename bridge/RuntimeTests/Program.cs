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
using log4net;
using RuntimeTests.RuntimeTests;

namespace RuntimeTests
{
    public class Program
    {
        private static ILog logger = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();
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

                try
                {
                    jms.TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithIcomingPort();
                    jms.TestACustomCreatedExceptionHandlerWhichReturnsAValueAsResultThatGetsInvokedWithOutgoingPort();
                    jms.TestJmsOutgoingPortWithQueueNameExceptionHandler();
                    jms.TestJmsPortWithWrongUrlParameter();
                    jms.CleanUp();
                    logger.Info("Jms tests completed correctly");
                }
                catch (Exception ex)
                {
                    logger.Error("Error", ex);
                }

                try
                {
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
                    net.CleanUp();
                    logger.Info(".Net Bridge tests completed correctly");
                }
                catch (Exception ex)
                {
                    logger.Error("Error", ex);
                }

                try
                {
                    bridge1.TestCreateDeleteConnectorAndNoRegistrationWorksCorrectlyWithUsernameAndPassword();
                    bridge1.TestCreateDeleteConnectorWithoutRegistrationWithUsernameAndPasswordWorksCorrectlyUsernameAndPassword();
                    bridge1.TestCreateRegisterEventHandlerUnregisterDeleteWorksCorrectlyWithUsernameAndPassword();
                    bridge1.TestCreateRegisterUnregisterDeleteConnectorWorksCorrectlyWithUsernameAndPassword();
                    bridge1.TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectlyWithUsernameAndPassword();
                    bridge1.CleanUp();
                    logger.Info(".Net Bridge tests completed correctly");
                }
                catch (Exception ex)
                {
                    logger.Error("Error", ex);                
                }

                try
                {
                    bridge2.TestCreateConnectorWithWrongUsernameAndPassword();
                    bridge2.TestRegisterConnectorWithoutCreateAndWithWrongUsernameAndPassword();
                    bridge2.CleanUp();
                    logger.Info(".Net Bridge tests completed correctly");
                }
                catch (Exception ex)
                {
                    logger.Error("Error", ex);
                }
             
                try
                {
                    xlink.TestCreateRegisterConnecttoxlinkDisconnectfromXlinkUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectly();
                    xlink.CleanUp();
                    logger.Info("Xlink tests completed correctly");
                }
                catch (Exception ex)
                {
                    logger.Error("Error", ex);
                }
            }
            finally
            {
                OSBRunTimeTestParent.CloseOSB();
            }
        }
    }
}