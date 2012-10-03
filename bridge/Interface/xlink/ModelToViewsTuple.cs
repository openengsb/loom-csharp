using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Org.Openengsb.Loom.CSharp.Bridge.Interface.Common.xlink
{
    [DataContractAttribute(Namespace="http://model.xlink.api.core.openengsb.org",Name="ModelToViewsTuple")]
    public class ModelToViewsTuple
    {


        /// <summary>
        /// Identifier of an OpenEngSBModel
        /// </summary>
        [DataMember]
        public ModelDescription description { get; set; }
        /// <summary>
        /// List of Views, offered by the remote tool
        /// </summary>
        [DataMember]
        public List<RemoteToolView> views { get; set; }

        public ModelToViewsTuple() { }
        public ModelToViewsTuple(ModelDescription description, List<RemoteToolView> views)
        {
            this.description = description;
            this.views = views;
        }
    }
}
