using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.ETM;

namespace Org.Openengsb.Loom.CSharp.Bridge.ETM.TCP
{
    /// <summary>
    /// Bind a InteractionMessage to a counter. This counter stand for the number of picks from this object
    /// </summary>
    class CountedInteractionMessage
    {
        /// <summary>
        /// Counter
        /// </summary>
        public int PickedNumber { get; set; }
        public InteractionMessage InteractionMessage { get; set; }
        /// <summary>
        /// Initialisation of the pickNumber with 0 and set InteractionMessage
        /// </summary>
        /// <param name="interactionMessage"></param>
        public CountedInteractionMessage(InteractionMessage interactionMessage)
        {
            this.PickedNumber = 0;
            this.InteractionMessage = interactionMessage;
        }
    }
}
