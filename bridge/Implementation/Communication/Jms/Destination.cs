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

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Communication.Jms
{
    /// <summary>
    /// Type for URLS
    /// </summary>
    public class Destination
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="destination">Url of the Destination</param>
        public Destination(string destination)
        {
            string[] parts = destination.Split('?');

            if (parts.Length != 2)
                throw new ApplicationException("JMSPort Destination string invalid!");

            Host = parts[0].Trim();
            Queue = parts[1].Trim();
        }
        #endregion
        #region Variables
        public string Host { get; set; }
        public string Queue { get; set; }
        #endregion
        #region Propreties
        /// <summary>
        /// Get the hole URL String
        /// </summary>
        public string FullDestination
        {
            get
            {
                if(string.IsNullOrEmpty(Queue))
                    return Host;
                return Host + "?" + Queue;
            }
        }
        #endregion        
        #region Public static Methods
        /// <summary>
        /// Combines host and queue
        /// </summary>
        /// <param name="host">Host</param>
        /// <param name="queue">Queue</param>
        /// <returns>Combination of the host and queue</returns>
        public static string CreateDestinationString(string host, string queue)
        {
            return host + "?" + queue;
        }
        #endregion
    }
}
