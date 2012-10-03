using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common.xlink
{
    public class XLinkTemplateKeyNames
    {
        [DataMember]
        /// <summary>
        ///  Keyname of the modelClass, which must be concatenated to the baseUrl 
        ///  GET Paramter
        /// </summary>
        public String modelClassKeyName { get; set; }
        [DataMember]
        /// <summary>
        /// Keyname of the version of the model, which must be concatenated to the baseUrl as 
        /// GET Paramter
        /// </summary>
        private String modelVersionKeyName { get; set; }
        [DataMember]
        /// <summary>
        /// Keyname of the identifierString of the model, which must be concatenated to the baseUrl as 
        /// GET Paramter
        /// </summary>
        public String identifierKeyName { get; set; }

        [DataMember]
        /// <summary>
        /// Keyname of the contextId, must be set by the tool to determine the OpenEngSB context of the XLink.
        /// To select the root context, ad this key with no value.
        /// </summary>
        public String contextIdKeyName { get; set; }
        [DataMember]
        /// <summary>
        /// Keyname of the viewId, which is to be used for local switching.
        /// Must be added, with corresponding value, to the baseUrl as GET-Paramter.
        /// </summary>
        public String viewIdKeyName { get; set; }

        public XLinkTemplateKeyNames()
        {

        }

        public XLinkTemplateKeyNames(String modelClassKeyName,
                String modelVersionKeyName, String identifierKeyName,
                String contextIdKeyName, String viewIdKeyName)
        {
            this.modelClassKeyName = modelClassKeyName;
            this.modelVersionKeyName = modelVersionKeyName;
            this.identifierKeyName = identifierKeyName;
            this.contextIdKeyName = contextIdKeyName;
            this.viewIdKeyName = viewIdKeyName;
        }

    }
}