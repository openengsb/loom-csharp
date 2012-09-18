﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using @event.example.domain.openengsb.org.xsd;
using model.example.domain.openengsb.org.xsd;

namespace ETMTest
{
    #region ExampleClass
    public class ExampleDomainConnector : RegistrationFunctions, ExampleDomainPortType
    {
        private static ILog logger = LogManager.GetLogger(typeof(ExampleDomainConnector));
        public ExampleDomainConnector() : base(logger) { }
        public string level { get; set; }
        public string message { get; set; }
        public string name { get; set; }
        public string origin { get; set; }
        public long? processId { get; set; }
        public string doSomethingWithLogEvent(LogEvent args0)
        {
            level = args0.level;
            message = args0.message;
            name=args0.name;
            origin= args0.origin;
            processId =args0.processId;
            //logger.Info("run doSomethingWithEnum with " + args0);
            return "DEBUG";
        }

        public string doSomethingWithMessage(string args0)
        {
            logger.Info("run doSomethingWithEnum with " + args0);
            return "Hallo";
        }

        public ExampleResponseModel doSomethingWithModel(ExampleRequestModel args0)
        {
            logger.Info("Message received in doSomethingWithModel");
            ExampleResponseModel m = new ExampleResponseModel();
            m.result = "RESULT";
            return m;
        }
    }
    #endregion
}
