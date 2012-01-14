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
using Apache.NMS;

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Implementation.Communication.Jms
{
    /// <summary>
    /// Send Message
    /// </summary>
    public class JmsOutgoingPort : JmsPort, IOutgoingPort
    {
        #region Variables
        IMessageProducer _producer;
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="destination">URL to OpenEngSB</param>
        public JmsOutgoingPort(string destination): base(destination)
        {
            _producer = _session.CreateProducer(_destination);
            _producer.DeliveryMode = MsgDeliveryMode.Persistent;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Send an string over NMS.
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="receiver">Queue name on server side</param>
        public void Send(string text)
        {
            ITextMessage message = _session.CreateTextMessage(text);
            _producer.Send(message);
        }

        /// <summary>
        /// Close the Connection
        /// </summary>
        public new void Close()
        {
            base.Close();
            _producer.Close();
        }
        #endregion
    }
}
