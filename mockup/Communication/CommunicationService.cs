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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;

namespace Org.OpenEngSB.DotNet.Lib.MockupMonitor.Communication
{
    public class CommunicationService
    {
        private static CommunicationService _me;

        private ServiceHost _selfHost;

        public static void StartService(IMonitorService serviceImpl)
        {
            if (_me == null)
            {
                _me = new CommunicationService();
                _me.DoStartService(serviceImpl);
            }
        }

        private void DoStartService(IMonitorService serviceImpl)
        {
            Uri baseAdress = new Uri("http://0.0.0.0:8033/OpenEngSB/DotNet/Service");

            _selfHost = new ServiceHost(serviceImpl, baseAdress);

            _selfHost.AddServiceEndpoint(typeof(IMonitorService), new WSDualHttpBinding(), "MonitorService");

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();

            smb.HttpGetEnabled = true;
            
            _selfHost.Description.Behaviors.Add(smb);
            
            foreach (var b in _selfHost.Description.Behaviors)
            {
                if (b is ServiceDebugBehavior)
                {
                    ((ServiceDebugBehavior)b).IncludeExceptionDetailInFaults = true;
                    break;
                }
            }

            _selfHost.Open();
        }

        public static void StopService()
        {
            if (_me != null)
            {
                _me.DoStopService();
                _me = null;
            }
        }

        private void DoStopService()
        {
            _selfHost.Close();
            _selfHost = null;
        }
    }
}
