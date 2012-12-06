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

using Apache.NMS;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;
using log4net;
using System;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms
{
    /// <summary>
    /// Receive the datas
    /// </summary>
    public class JmsIncomingPort : JmsPort, IIncomingPort
    {
        protected static ILog logger = LogManager.GetLogger(typeof(JmsIncomingPort));

        #region variables
        IMessageConsumer consumer;
        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="destination">URL to OpenEngSB</param>
        public JmsIncomingPort(string destination, EExceptionHandling exceptionhandling)
            : base(destination,exceptionhandling)
        {
            consumer = session.CreateConsumer(this.destination);
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Waits for a message on the preconfigured queue.
        /// Blocks until a message in received or the connection is closed.
        /// </summary>
        /// <returns>Read message. Null if the connection is closed.</returns>
        public string Receive()
        {
            ITextMessage message = null;
            try
            {
                logger.Info("wait for new message");
                message = consumer.Receive() as ITextMessage;
                logger.Info("recieved new message. Processing...");
            }
            catch (Exception e)
            {
                logger.WarnFormat("Exception caught in receivethread. Maybe OpenEngSB terminated - {0} ({1}).", e.Message, e.GetType().Name);
                if (!close)
                {
                    logger.Warn("trying to reconnect");
                    Configure();
                    consumer = session.CreateConsumer(this.destination);
                    logger.Warn("configuration successful");
                }
                throw e;
            }

            if (message == null)
                return null;
            logger.DebugFormat("recieved message: {0}", message.Text);
            return message.Text;
        }
        /// <summary>
        /// Close the Connection
        /// </summary>
        public new void Close()
        {
            consumer.Close();
            base.Close();
        }
        #endregion

    }
}
