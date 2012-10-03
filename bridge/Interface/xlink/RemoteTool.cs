using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.xlink
{
    public class RemoteTool
    {
        /// <summary>
        /// Id of the connector, identifying the tool
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// Human readable name of the tool, may be null
        /// </summary>
        public String ToolName { get; set; }
        /// <summary>
        /// Views the tool offers for XLink, represented as keyNames and short descriptions
        /// </summary>
        public List<RemoteToolView> AvailableViews { get; set; }

        public RemoteTool() { }
        public RemoteTool(String Id, String ToolName, List<RemoteToolView> AvailableViews)
        {
            this.Id = Id;
            this.ToolName = ToolName;
            this.AvailableViews = AvailableViews;
        }
    }
}
