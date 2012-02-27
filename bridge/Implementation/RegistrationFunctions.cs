using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Bridge.Implementation
{
    public partial class RegistrationFunctions
    {
        private ILog logger;
        public RegistrationFunctions(ILog logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void setDomainId(String element)
        {
            logger.Info("setDomainId:" + element);
        }
        /// <summary>
        /// Part of the registration process
        /// </summary>
        /// <param name="element">Message from the server</param>
        public void setConnectorId(String element)
        {
            logger.Info("setConnectorId:" + element);
        }
    }
}
