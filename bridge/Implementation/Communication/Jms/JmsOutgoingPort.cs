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
using Apache.NMS;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;
using System.Reflection;
namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms
{
    /// <summary>
    /// Send Message
    /// </summary>
    public class JmsOutgoingPort : JmsPort, IOutgoingPort
    {
        #region Variables
        private IMessageProducer producer;
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="destination">URL to OpenEngSB</param>
        public JmsOutgoingPort(string destination, ABridgeExceptionHandling handling, String connectorId)
            : base(destination, handling, connectorId)
        {
            producer = Session.CreateProducer(this.Destination);
            producer.DeliveryMode = MsgDeliveryMode.Persistent;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Send a string over NMS.
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="receiver">Queue name on server side</param>
        public void Send(string text)
        {
            try
            {
                ITextMessage message = Session.CreateTextMessage(text);
                producer.Send(message);
            }
            catch (Exception ex)
            {
                Handling.Changed += (delegate(object[] obj)
                {
                    Send(obj[0].ToString());
                    return null;
                });
                Handling.HandleException(ex, text);
            }
        }

        /// <summary>
        /// Send a string over NMS and defines the replyTo field
        /// </summary>
        /// <param name="text">Text to send</param>
        /// <param name="replyTo">Reply destination</param>
        public void Send(string text, String queueName)
        {
            try
            {
                ITextMessage message = Session.CreateTextMessage(text);
                message.NMSReplyTo = Session.GetQueue(queueName);
                producer.Send(message);
            }
            catch (Exception ex)
            {
                Handling.Changed += (delegate(object[] obj)
                {
                    Send(obj[0].ToString(), obj[1].ToString());
                    return null;
                });
                Handling.HandleException(ex, text, queueName);
            }
        }

        /// <summary>
        /// Close the Connection
        /// </summary>
        public new void Close()
        {
            producer.Close();
            producer.Dispose();
            base.Close();
        }
        #endregion
    }
}