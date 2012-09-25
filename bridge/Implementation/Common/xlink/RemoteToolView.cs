using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.xlink
{
    public class RemoteToolView
    {
        /// <summary>
        /// Unique Id of the View 
        /// </summary>
        public String ViewId { get; set; }
        /// <summary>
        /// Human readable name of the view 
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Map with locale strings as key (such as "en" and "de") and an description of the
        /// view in the specified language. Implementation must make sure that a default 
        /// value is returned if a locale is not contained. If the system encounters
        /// a null-value for a certain locale, the first entry of the map is taken 
        /// instead.
        /// </summary>
        public Dictionary<String, String> Descriptions { get; set; }
        public RemoteToolView() { }
        public RemoteToolView(String ViewId, String Name, Dictionary<String, String> Descriptions)
        {
            this.ViewId = ViewId;
            this.Name = Name;
            this.Descriptions = Descriptions;
        }

    }
}