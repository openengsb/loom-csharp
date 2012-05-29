using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Implementation.OpenEngSB3_0_0.Remote.RemoteObjects
{
    public abstract class MessageBase
    {       
        public long timestamp { get; set; }
        public string callId { get; set; }       
    }
}
