using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using OOSourceCodeDomain;
using BridgeTests.TestConnectorImplementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using OpenEngSBCore;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
namespace BridgeTests
{
    [TestClass]
    [ExcludeFromCodeCoverageAttribute()]
    public class TestXLinkConnection
    {
        private IDomainFactory factory;
        private String uuid;
        private const String domainName = "oosourcecode";
        private const String destination = "tcp://localhost.:6549";
        [TestInitialize]
        public void InitialiseFactory()
        {
            IOOSourceCodeDomainSoap11Binding exampleDomain = new OOSourceCodeDomainConnector();
            factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, exampleDomain, new ForwardDefaultExceptionHandler());
        }
        [TestMethod]
        public void TestCreateRegisterUnregisterDeleteWithoutCreateMethodConnector()
        {
            uuid = factory.RegisterConnector(null, domainName);

            Assert.IsTrue(factory.Registered(uuid));
            Assert.IsFalse(factory.Registered("WRONG ID"));
            Assert.IsTrue(factory.GetDomainTypConnectorId(uuid).Equals(domainName + "+external-connector-proxy+" + uuid));
            
            XLinkUrlBlueprint template = factory.ConnectToXLink(uuid, "localhost", domainName, initModelViewRelation());
            factory.DisconnectFromXLink(uuid, "localhost");
            factory.UnRegisterConnector(uuid);
            
            Assert.IsFalse(factory.Registered(uuid));
            
            factory.DeleteDomainService(uuid);
            
            Assert.IsFalse(factory.Registered(uuid));
        }

        private ModelToViewsTuple[] initModelViewRelation()
        {
            ModelToViewsTuple[] modelsToViews = new ModelToViewsTuple[1];
            Dictionary<String, String> descriptions = new Dictionary<String, String>();
            descriptions.Add("en", "This view opens the values in a SQLViewer.");
            descriptions.Add("de", "Dieses Tool öffnet die Werte in einem SQLViewer.");
            OpenEngSBCore.XLinkConnectorView[] views = new OpenEngSBCore.XLinkConnectorView[1];
            views[0] = (new OpenEngSBCore.XLinkConnectorView() { name = "SQLView", viewId = "SQL Viewer", descriptions = descriptions.ConvertMap<entry3>() });
            modelsToViews[0] =
                    new ModelToViewsTuple()
                    {
                        description = new ModelDescription() { modelClassName = "org.openengsb.domain.SQLCode.model.SQLCreate", versionString = "3.0.0.SNAPSHOT" },
                        views = views
                    };
            return modelsToViews;
        }
        
        [TestCleanup]
        public void CleanUp()
        {
            factory.StopConnection(uuid);
        }
    }
}