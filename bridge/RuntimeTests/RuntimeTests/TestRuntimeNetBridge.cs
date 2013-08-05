using System;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using ExampleDomain;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using RuntimeTests.TestConnectorImplementation;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace RuntimeTests.RuntimeTests
{
    public class TestRuntimeNetBridge : OSBRunTimeTestParent
    {
        private IDomainFactory factory;
        private const String version = "3.0.0";
        private const String destination = "tcp://localhost.:6549";
        private const String domainName = "example";
        private const String nullString = null;
        private ABridgeExceptionHandling exceptionHandler;
        private String uuid;

        public override void Init()
        {
            exceptionHandler = new RetryDefaultExceptionHandler();
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, exampleDomain, exceptionHandler);
        }

        public void TestCreateDeleteConnectorAndNoRegistrationWorksCorrectly()
        {
            uuid = factory.CreateDomainService(domainName);

            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));

            factory.DeleteDomainService(uuid);
        }

        public void TestCreateDeleteConnectorWithoutRegistrationWithUsernameAndPasswordWorksCorrectly()
        {
            uuid = factory.CreateDomainService(domainName);

            Assert.IsFalse(factory.Registered(uuid));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));

            factory.DeleteDomainService(uuid);
        }

        public void TestCreateRegisterUnregisterDeleteConnectorWorksCorrectly()
        {
            uuid = factory.CreateDomainService(domainName);
            factory.RegisterConnector(uuid, domainName);

            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));

            factory.UnRegisterConnector(uuid);

            Assert.IsFalse(factory.Registered(uuid));

            factory.DeleteDomainService(uuid);

            Assert.IsFalse(factory.Registered(uuid));
        }

        public void TestCreateRegisterConnectXlinkDisconnectXlinkUnregisterDeleteConnectorOnADomainThatDoesNotSupportXlink()
        {
            uuid = factory.CreateDomainService(domainName);
            factory.RegisterConnector(uuid, domainName);

            try
            {
                //Exampledomain does not support xlink => should throw OpenEngSBException
                factory.ConnectToXLink(uuid, "localhost", "example", new OpenEngSBCore.ModelToViewsTuple[1]);
                Assert.Fail();
            }
            catch (OpenEngSBException)
            {
            }
            factory.UnRegisterConnector(uuid);
            factory.DeleteDomainService(uuid);

        }

        public void TestConnectToXlinkWithNoncreatedAndNonregisteredConnectorAndCheckTheCorrectException()
        {
            uuid = "exampleId";
            try
            {
                factory.ConnectToXLink(uuid, nullString, domainName, null);
            }
            catch (BridgeException ex)
            {
                Assert.AreEqual<String>("The connecotr with id " + uuid + " has no instance", ex.Message);
            }
        }

        public void TestConnectToXlinkWithAConnectorThatIsNotregisteredConnectorAndAnalyseTheException()
        {
            uuid = factory.CreateDomainService("example");
            try
            {

                factory.ConnectToXLink(uuid, "localhost", "example", null);
            }
            catch (BridgeException ex)
            {
                Assert.AreEqual<String>(ex.Message, "The connecotr with id " + uuid + " has no instance");
            }
            finally
            {
                factory.DeleteDomainService(uuid);
            }


        }

        public void TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnectorWorksCorrectly()
        {
            uuid = factory.RegisterConnector(null, domainName);

            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));

            factory.UnRegisterConnector(uuid);

            Assert.IsFalse(factory.Registered(uuid));

            factory.DeleteDomainService(uuid);

            Assert.IsFalse(factory.Registered(uuid));
        }

        public void TestCreateRegisterWithEventHandlerInvokedUnregisterDeleteWorksCorrectly()
        {
            LogEvent logEvent = new LogEvent();
            logEvent.level = "1";
            logEvent.message = "TestCase";
            logEvent.name = "Test";

            uuid = factory.RegisterConnector(null, domainName);
            IExampleDomainEventsSoapBinding exampleDomain = factory.GetEventhandler<IExampleDomainEventsSoapBinding>(uuid);

            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            Assert.IsNotNull(exampleDomain);

            exampleDomain.raiseEvent(logEvent);
            factory.UnRegisterConnector(uuid);

            Assert.IsFalse(factory.Registered(uuid));

            factory.DeleteDomainService(uuid);

            Assert.IsFalse(factory.Registered(uuid));
        }

        public void TestCreateRegisterConnectorAndGetEventHandlerWithInvalitConnectorId()
        {
            try
            {
                uuid = factory.RegisterConnector(nullString, domainName);
                IExampleDomainEventsSoapBinding exampleDomain = factory.GetEventhandler<IExampleDomainEventsSoapBinding>(nullString);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }
        }

        public void TestCreateMoreConnectorAndStopAllWorksCorrectly()
        {
            String[] uuids = new String[10];
            for (int i = 0; i < uuids.Length; i++)
            {
                uuid = factory.CreateDomainService(domainName);
                uuids[i] = uuid;
                factory.RegisterConnector(uuid, domainName);
                factory.UnRegisterConnector(uuid);
                factory.DeleteDomainService(uuid);
            }
            factory.StopAllConnections();

            foreach (String tmpuuid in uuids)
            {
                //Test if there is an connector that exists;
                try
                {
                    factory.GetDomainTypConnectorId(tmpuuid);
                    Assert.Fail();
                }
                catch (BridgeException ex)
                {
                    Assert.AreEqual<String>("There is no connector with the connectorId " + tmpuuid, ex.Message);
                }
            }
        }

        public void TestCreateRegisterCloseRegisterWorksCorrectly()
        {
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            IDomainFactory factory = DomainFactoryProvider.GetDomainFactoryInstance(version, destination, exampleDomain, exceptionHandler);
            String tmpuuid = null;
            uuid = factory.CreateDomainService(domainName);
            factory.RegisterConnector(uuid, domainName);
            factory.StopConnection(uuid);
            tmpuuid = uuid;
            factory.RegisterConnector(uuid, domainName);
            factory.StopConnection(uuid);
            Assert.AreEqual<String>(tmpuuid, uuid);
        }

        public override void CleanUp()
        {
            exceptionHandler.Stop = true;
            factory.StopAllConnections();
        }
    }
}