using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.xlink
{
    public class ModelToViewsTuple
    {


        /// <summary>
        /// Identifier of an OpenEngSBModel
        /// </summary>
        public ModelDescription description { get; set; }
        /// <summary>
        /// List of Views, offered by the remote tool
        /// </summary>
        public List<RemoteToolView> views { get; set; }

        public ModelToViewsTuple() { }
        public ModelToViewsTuple(ModelDescription description, List<RemoteToolView> views)
        {
            this.description = description;
            this.views = views;
        }
    }
}
