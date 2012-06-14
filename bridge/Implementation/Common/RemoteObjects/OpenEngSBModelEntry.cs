using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Implementation.Common.RemoteObjects
{
    public class OpenEngSBModelEntry
    {
        public String key { get; set; }
        public String value { get; set; }
        public String type { get; set; }
        public OpenEngSBModelEntry() { }
        public static OpenEngSBModelEntry getInstance(String key, String value, String type)
        {
            OpenEngSBModelEntry result = new OpenEngSBModelEntry();
            result.key = key;
            result.value = value;
            result.type = type;
            return result;
        }
    }
}
