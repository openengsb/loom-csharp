using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication.Json
{
    public class CustomJsonMarshaller : JsonConverter 
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Name.ToUpper().Contains("ENTRY");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IDictionary list = new Dictionary<Object,Object>();
            serializer.Populate(reader, list);            
            return list.ConvertMap(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Object tmp = value.ConvertMap();
            serializer.Serialize(writer, tmp);
        }
    }
}
