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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation
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
        public static IDomainFactory GetDomainFactoryInstance<T>(String stringVersion, String destination, T service)
        {
            try
            {
                String versionnbr = stringVersion.Replace(".", "");
                Regex rgx = new Regex("-.*");
                versionnbr = rgx.Replace(versionnbr, "");
                int version = int.Parse(versionnbr);
                if (version >= 300)
                    return new Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.RealDomainFactory<T>(destination, service);
                if (version >= 240)
                    return new Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB2_4_0.RealDomainFactory<T>(destination, service);
                return null;
            }
            catch
            {
                throw new BridgeException("Unable to receive the actually version from: " + stringVersion);
            }
        }
        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="urlVersion">Version of the OpenEngSB-framework in url format</param>
        /// <returns>factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<T>(Uri urlVersion, String destination, T service)
        {
            Uri uri = urlVersion;
            if (!uri.ToString().Contains("system/framework.version.info"))
                uri = new Uri(uri.ToString() + "system/framework.version.info");
            WebClient myWebClient = new WebClient();
            Byte[] myDataBuffer = myWebClient.DownloadData(uri);
            String stringVersion = Encoding.ASCII.GetString(myDataBuffer);
            return GetDomainFactoryInstance(stringVersion, destination, service);
        }
    }
}