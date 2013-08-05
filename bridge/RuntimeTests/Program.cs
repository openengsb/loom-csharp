using RuntimeTests;
using RuntimeTests.RuntimeTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeTests
{
    class Program
    {
        static void Main(string[] args)
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