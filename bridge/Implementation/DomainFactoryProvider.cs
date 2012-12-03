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
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

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
                int version=getVersionNumber(stringVersion);
                if (version >= 300)
                    return new Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.RealDomainFactory<T>(destination, service);
                if (version >= 240)
                    return new Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB2_4_0.RealDomainFactory<T>(destination, service);
                return null;
        }
        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="stringVersion">Version of the OpenEngSB-framework in String format</param>
        /// <returns>Factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<T>(String stringVersion, String destination, T service, ABridgeExceptionHandling exceptionhandler)
        {
                int version = getVersionNumber(stringVersion);
                if (version >= 300)
                    return new Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB3_0_0.RealDomainFactory<T>(destination, service,exceptionhandler);
                if (version >= 240)
                    return new Org.Openengsb.Loom.CSharp.Bridge.Implementation.OpenEngSB2_4_0.RealDomainFactory<T>(destination, service,exceptionhandler);
                return null;
        }
        /// <summary>
        /// Parse th a string and filters the version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static int getVersionNumber(String version)
        {
            try
            {
                String versionnbr = version.Replace(".", "");
                Regex rgx = new Regex("-.*");
                versionnbr = rgx.Replace(versionnbr, "");
                return int.Parse(versionnbr);
            }
            catch
            {
                throw new BridgeException("Unable to receive the actually version from: " + version);
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