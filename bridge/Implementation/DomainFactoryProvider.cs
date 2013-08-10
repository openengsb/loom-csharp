#region Copyright
// <copyright file="DomainFactoryProvider.cs" company="OpenEngSB">
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
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation
{
    /// <summary>
    /// Factory Provider
    /// </summary>
    public class DomainFactoryProvider
    {
        #region Private Static Variables
        private static String startingNameOfOpenEngSBAssemblies = "OpenEngSB";
        private static List<Assembly> supportedOpenEngSBVersions = new List<Assembly>();
        #endregion
        #region Public Methods
        /// <summary>
        /// Allos it to define the supported versions
        /// </summary>
        /// <param name="openengsbAssembly"></param>
        public static void AddSupport(Assembly openengsbAssembly)
        {
            supportedOpenEngSBVersions.Add(openengsbAssembly);
        }

        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="stringVersion">Version of the OpenEngSB-framework in String format</param>
        /// <returns>Factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<ServiceTyp>(String stringVersion, String destination, ServiceTyp service)
        {
            Type domainResult = GetRealDomainFactory(stringVersion);
            domainResult = domainResult.MakeGenericType(typeof(ServiceTyp));
            return Activator.CreateInstance(domainResult, destination, service) as IDomainFactory;
        }

        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="stringVersion">Version of the OpenEngSB-framework in String format</param>
        /// <returns>Factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<ServiceTyp>(String stringVersion, String destination, ServiceTyp service, ABridgeExceptionHandling exceptionhandler)
        {
            Type domainResult = GetRealDomainFactory(stringVersion);
            domainResult = domainResult.MakeGenericType(typeof(ServiceTyp));
            return Activator.CreateInstance(domainResult, destination, service, exceptionhandler) as IDomainFactory;
        }

        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="stringVersion">Version of the OpenEngSB-framework in String format</param>
        /// <returns>Factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<ServiceTyp>(String stringVersion, String destination, ServiceTyp service, String username, String password)
        {
            Type domainResult = GetRealDomainFactory(stringVersion);
            domainResult = domainResult.MakeGenericType(typeof(ServiceTyp));
            return Activator.CreateInstance(domainResult, destination, service, username, password) as IDomainFactory;
        }

        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="stringVersion">Version of the OpenEngSB-framework in String format</param>
        /// <returns>Factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<ServiceTyp>(String stringVersion, String destination, ServiceTyp service, ABridgeExceptionHandling exceptionhandler, String username, String password)
        {
            Type domainResult = GetRealDomainFactory(stringVersion);
            domainResult = domainResult.MakeGenericType(typeof(ServiceTyp));
            return Activator.CreateInstance(domainResult, destination, service, exceptionhandler, username, password) as IDomainFactory;
        }

        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="urlVersion">Version of the OpenEngSB-framework in url format</param>
        /// <returns>factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<ServiceTyp>(Uri urlVersion, String destination, ServiceTyp service)
        {
            return GetDomainFactoryInstance(GetVersionFromURI(urlVersion), destination, service);
        }

        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="urlVersion">Version of the OpenEngSB-framework in url format</param>
        /// <returns>factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<ServiceTyp>(Uri urlVersion, String destination, ServiceTyp service, String username, String password)
        {
            return GetDomainFactoryInstance(GetVersionFromURI(urlVersion), destination, service, username, password);
        }

        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="urlVersion">Version of the OpenEngSB-framework in url format</param>
        /// <returns>factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<T>(Uri urlVersion, String destination, T service, ABridgeExceptionHandling exceptionHandler)
        {
            return GetDomainFactoryInstance(GetVersionFromURI(urlVersion), destination, service, exceptionHandler);
        }

        /// <summary>
        /// Retrieve a factory, depending on the openEngSB version
        /// </summary>
        /// <param name="urlVersion">Version of the OpenEngSB-framework in url format</param>
        /// <returns>factory</returns>
        public static IDomainFactory GetDomainFactoryInstance<T>(Uri urlVersion, String destination, T service, ABridgeExceptionHandling exceptionhandler, String username, String password)
        {
            return GetDomainFactoryInstance(GetVersionFromURI(urlVersion), destination, service, exceptionhandler, username, password);
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Seach in all dlls of the current Solution for the OpenEnGSB version
        /// </summary>
        /// <returns>The type of the RealDomainFactory</returns>
        private static Type GetRealDomainFactory(String version)
        {
            try
            {
                Assembly osbassembly = SearchAssemblyInSupportedList(version);
                if (osbassembly == null)
                {
                    osbassembly = Assembly.Load(startingNameOfOpenEngSBAssemblies + version);
                }

                List<Type> types = new List<Type>(osbassembly.GetTypes());
                Type domainResult = types.Find(tmptype => typeof(IDomainFactory).IsAssignableFrom(tmptype));
                return domainResult;
            }
            catch (Exception ex)
            {
                throw new BridgeException(
                                          "There could not Assembly with the Name OpenEngSB" +
                                          version +
                                          " be found. Maybe you did not add this assembly to the solution or did" +
                                          " not invoke the support method from the OpenEngSB implementation",
                                          ex);
            }
        }

        private static string GetVersionFromURI(Uri urlVersion)
        {
            Uri uri = urlVersion;
            if (!uri.ToString().Contains("system/framework.version.info"))
            {
                uri = new Uri(uri.ToString() + "system/framework.version.info");
            }

            WebClient myWebClient = new WebClient();
            Byte[] myDataBuffer = myWebClient.DownloadData(uri);
            String stringVersion = Encoding.ASCII.GetString(myDataBuffer);
            return stringVersion;
        }

        /// <summary>
        /// Parse th a string and filters the version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static int GetVersionNumber(String version)
        {
            try
            {
                String versionnbr = version.Replace(".", String.Empty);
                Regex rgx = new Regex("-.*");
                versionnbr = rgx.Replace(versionnbr, String.Empty);
                return int.Parse(versionnbr);
            }
            catch
            {
                throw new BridgeException("Unable to receive the actually version from: " + version);
            }
        }

        /// <summary>
        /// Searchs throw all the supported elements in the List and search for the correct version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static Assembly SearchAssemblyInSupportedList(String version)
        {
            Version t = new Version(version);
            foreach (Assembly ass in supportedOpenEngSBVersions)
            {
                Version v = ass.GetName().Version;
                int b = v.Build;
                if (v.Major == t.Major && v.Minor == t.Minor && v.Build == t.Build)
                {
                    return ass;
                }
            }

            return null;
        }
        #endregion
    }
}