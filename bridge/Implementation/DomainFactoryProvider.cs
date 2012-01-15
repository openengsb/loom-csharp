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
using System.IO;
using Org.OpenEngSB.Loom.Csharp.Common.Bridge.Interface;
using Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Exceptions;
using System.Net;

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation
{
    /// <summary>
    /// Factory Provider
    /// </summary>
    public class DomainFactoryProvider
    {
        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="stringVersion">Version of the OpenEngSB-framework in String format</param>
        /// <returns>Factory</returns>
        public static IDomainFactory GetDomainFactoryInstance(String stringVersion)
        {
            try
            {
                String versionnbr = stringVersion.Replace(".", "");
                versionnbr = versionnbr.Replace("-SNAPSHOT", "");
                int version = int.Parse(versionnbr);
                if (version >= 300) return new Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.OpenEngSB3_0_0.RealDomainFactory();
                if (version >= 240) return new Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.OpenEngSB2_4_0.RealDomainFactory();
                return null;
            }
            catch 
            {
                throw new ConnectOpenEngSBException("Unable to receive the actually version from: "+stringVersion);
            }
        }
        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="urlVersion">Version of the OpenEngSB-framework in url format</param>
        /// <returns>factory</returns>
        public static IDomainFactory GetDomainFactoryInstance(Uri urlVersion)
        {
            Uri uri = urlVersion;
            if (!uri.ToString().Contains("system/framework.version.info")) uri = new Uri(uri.ToString() + "system/framework.version.info");
            WebClient myWebClient = new WebClient();
            Byte[] myDataBuffer = myWebClient.DownloadData(uri);
            String stringVersion = Encoding.ASCII.GetString(myDataBuffer);
            return GetDomainFactoryInstance(stringVersion);
        }
    }
}
