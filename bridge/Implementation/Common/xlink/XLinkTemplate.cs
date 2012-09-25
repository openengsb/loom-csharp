﻿    /**
 * Licensed to the Austrian Association for Software Tool Integration (AASTI)
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. The AASTI licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.xlink
{

    public class XLinkTemplate
    {
        /// <summary>
        ///  URL to the Registry´s HTTP-Servlet without necessary GET-Parameters. 
        ///  Already contains the expirationDate of the Link as GET-Parameter. 
        ///  XLink-URLs expire after a certain amount of days.
        /// </summary>
        public String baseUrl { get; set; }

        /// <summary>
        /// Map with the available viewId as key and the assigned modelclass as value.
        /// This Map defines which Model is to be used for which view.
        /// </summary>
        public IDictionary<String, ModelDescription> viewToModels { get; set; }

        /// <summary>
        /// List of all other currently registered tools from the same host. 
        /// This list of registered tools can be used to support 
        /// 'local-switching' between local tools.
        /// </summary>
        public IList<RemoteTool> registeredTools { get; set; }

        /// <summary>
        /// Key/value combination of the connectorId in HTTP GET paramater syntax.
        /// Must be concatenated to the baseUrl, when the generated XLink should 
        /// to be used for 'local-switching'.
        /// </summary>
        public String connectorId { get; set; }

        /// <summary>
        /// Contains a set of Keynames, which are to be used for constructing valid XLinkURLs.
        /// </summary>
        private XLinkTemplateKeyNames keyNames;

        public XLinkTemplate() { }
        public XLinkTemplate(String baseUrl, IDictionary<String, ModelDescription> viewToModels,
                List<RemoteTool> registeredTools, String connectorId,
                XLinkTemplateKeyNames keyNames)
        {
            this.baseUrl = baseUrl;
            this.viewToModels = viewToModels;
            this.registeredTools = registeredTools;
            this.connectorId = connectorId;
            this.keyNames = keyNames;
        }
    }
}