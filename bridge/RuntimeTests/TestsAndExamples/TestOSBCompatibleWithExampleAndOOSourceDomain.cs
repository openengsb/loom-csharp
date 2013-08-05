using System;
using RuntimeTests.TestConnectorImplementation;
using Org.Openengsb.Loom.CSharp.Bridge.OpenEngSB300;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using OpenEngSBCore;
using ExampleDomain;
using System.Collections.Generic;

namespace RuntimeTests.TestsAndExamples
{
    
    public class TestOSBCompatibleWithExampleAndOOSourceDomain : OSBRunTimeTestParent
    {

        public override void Init()
        {
        }
        public void TestExampleCodeExecution()
        {
            Boolean xlink = false;
            ExampleDomainConnector exampleDomain = new ExampleDomainConnector();
            OOSourceCodeDomainConnector ooconnector = new OOSourceCodeDomainConnector();
            OpenEngSB300.SetSupport();
            IDomainFactory factory;
            string domainName;
            string destination = "tcp://localhost.:6549";
            if (xlink)
            {
                //if you are using xlink for the example, please use an other domain. Example domain is not linkable
                domainName = "oosourcecode";
                factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, ooconnector, new RetryDefaultExceptionHandler());
            }
            else
            {
                domainName = "example";
                factory = DomainFactoryProvider.GetDomainFactoryInstance("3.0.0", destination, exampleDomain, new ForwardDefaultExceptionHandler());
            }

            String ConnectorId = factory.CreateDomainService(domainName);
            factory.RegisterConnector(ConnectorId, domainName);
            if (xlink)
            {
                XLinkUrlBlueprint template = factory.ConnectToXLink(ConnectorId, "localhost", domainName, initModelViewRelation());
                factory.DisconnectFromXLink(ConnectorId, "localhost");
            }
            else
            {
                IExampleDomainEventsSoapBinding remotedomain = factory.GetEventhandler<IExampleDomainEventsSoapBinding>(ConnectorId);
                LogEvent lEvent = new LogEvent();
                lEvent.name = "Example";
                lEvent.level = "DEBUG";
                lEvent.message = "remoteTestEventLog";
                remotedomain.raiseEvent(lEvent);
            }
            factory.UnRegisterConnector(ConnectorId);
            factory.DeleteDomainService(ConnectorId);
            factory.StopConnection(ConnectorId);
        }

        private ModelToViewsTuple[] initModelViewRelation()
        {
            ModelToViewsTuple[] modelsToViews = new ModelToViewsTuple[1];
            Dictionary<String, String> descriptions = new Dictionary<String, String>();
            descriptions.Add("en", "This view opens the values in a SQLViewer.");
            descriptions.Add("de", "Dieses Tool öffnet die Werte in einem SQLViewer.");
            XLinkConnectorView[] views = new XLinkConnectorView[1];
            views[0] = (new XLinkConnectorView() { name = "SQLView", viewId = "SQL Viewer", descriptions = descriptions.ConvertMap<string2stringMapEntry>() });
            modelsToViews[0] =
                    new ModelToViewsTuple()
                    {
                        description = new ModelDescription() { modelClassName = "org.openengsb.domain.SQLCode.model.SQLCreate", versionString = "3.0.0.SNAPSHOT" },
                        views = views
                    };
            return modelsToViews;
        }
        public override void CleanUp()
        {
        }

    }
}