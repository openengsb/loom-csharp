using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Implementation.Common.RemoteObjects
{
    public class OpenEngSBModelWrapper
    {
        public String modelClass{get;set;}
        public IList<OpenEngSBModelEntry> entries { get; set; }

        public OpenEngSBModelWrapper() { }

        public static OpenEngSBModelWrapper getInstance(String modelClass, IList<OpenEngSBModelEntry> entries)
        {
            OpenEngSBModelWrapper result = new OpenEngSBModelWrapper();
            result.modelClass = modelClass;
            result.entries = entries;
            return result;
        }
    }
}
