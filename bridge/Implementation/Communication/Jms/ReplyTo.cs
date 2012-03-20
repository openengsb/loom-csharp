using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.NMS;

namespace Implementation.Communication.Jms
{
    class ReplyTo:IDestination
    {
        private DestinationType desination;
        private bool isQueue;
        private bool isTemporary;
        private bool isTopic;

        public ReplyTo(DestinationType desination, bool isQueue, bool isTemporary, bool isTopic)
        {
            this.desination = desination;
            this.isQueue = isQueue;
            this.isTemporary = isTemporary;
            this.isTopic = isTopic;
        }
        public DestinationType DestinationType
        {
            get {return this.desination;}
            set { this.desination = value; }
        }

        public bool IsQueue
        {
            get { return isQueue; }
            set { this.isQueue = value; }
        }

        public bool IsTemporary
        {
            get { return this.isTemporary; }
            set { this.isTemporary = value; }
        }

        public bool IsTopic
        {
            get { return this.isTopic; }
            set { this.isTopic = value; }
        }
    }
}
