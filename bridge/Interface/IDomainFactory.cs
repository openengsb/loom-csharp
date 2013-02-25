/***
 * Licensed to the Austrian Association for Software Tool Integration (AASTI)
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. The AASTI licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 ***/
using System;
using OpenEngSBCore;

namespace Org.Openengsb.Loom.CSharp.Bridge.Interface
{
    public interface IDomainFactory
    {
        A GetEventhandler<A>(String connectorId);
        String CreateDomainService(String domainName);
        void DeleteDomainService(String connectorId);
        void RegisterConnector(String connectorId, String domainName);
        void UnRegisterConnector(String connectorId);
        XLinkUrlBlueprint ConnectToXLink(String connectorId, String hostId, String domainName, ModelToViewsTuple[] modelsToViews);
        void DisconnectFromXLink(String connectorId, String hostId);
        void StopConnection(String connectorId);
        String GetDomainTypConnectorId(String connectorId);
        Boolean Registered(String domainService);
    }
}