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

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Jms
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
