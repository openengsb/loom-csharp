#region Copyright
// <copyright file="OOSourceCodeDomainConnector.cs" company="OpenEngSB">
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
using OOSourceCodeDomain;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;

namespace ServiceTestConsole
{
    /// <summary>
    /// Example implementation of the local domain
    /// </summary>
    public class OOSourceCodeDomainConnector : RegistrationFunctions, IOOSourceCodeDomainSoap11Binding
    {
        #region Logger
        private static ILog logger = LogManager.GetLogger(typeof(ExampleDomainConnector));
        #endregion
        #region Constructors
        public OOSourceCodeDomainConnector()
            : base(logger)
        {
        }
        #endregion
        #region Public Methods
        public void updateClass(OOClass args0)
        {
            logger.Info("updateClasse method call");
        }

        public void getAliveState(out orgopenengsbcoreapiAliveState? @return, out bool returnSpecified)
        {
            @return = orgopenengsbcoreapiAliveState.ONLINE;
            returnSpecified = true;
            GetAliveState();
        }

        public string getInstanceId()
        {
            return ".Net Bridge connector";
        }

        public void onRegisteredToolsChanged(XLinkConnector[] args0)
        {
            logger.Info("onRegisteredToolsChanged method call");
        }

        public void openXLinks(object[] args0, string args1)
        {
            logger.Info("openXLinks method call");
        }
        #endregion
    }
}