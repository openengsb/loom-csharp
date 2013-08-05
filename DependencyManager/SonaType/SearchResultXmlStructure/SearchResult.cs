using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sonatype.SearchResultXmlStructure
{
    [Serializable]
    [XmlType("search-results")]
    public class SearchResult : System.Object
    {
        [XmlElement("totalCount")]
        public int TotalCount { get; set; }
        [XmlElement("from")]
        public String From { get; set; }
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlElement("tooManyResults")]
        public Boolean TooManyResults { get; set; }
        [XmlArray("data")]
        public List<Artifact> artefacts { get; set; }

        public SearchResult()
        {
            artefacts = new List<Artifact>();
        }
        public override Boolean Equals(Object artifact1)
        {
            if (!artifact1.GetType().Equals(this.GetType()))
            {
                return false;
            }
            PropertyInfo[] fields = this.GetType().GetProperties();
            foreach (PropertyInfo field in fields)
            {
                if (AreNotEqual(field.GetValue(this), field.GetValue(artifact1)))
                {
                    return false;
                }
            }
            return true;
        }

        private Boolean AreNotEqual(Object obj1Value, Object obj2Value)
        {
            if (obj2Value != null && isNotEqual(obj2Value, obj1Value))
            {
                return true;
            }
            else if (obj1Value != null && isNotEqual(obj1Value, obj2Value))
            {
                return true;
            }
            return false;
        }
        public Boolean isNotEqual(Object obj1, Object obj2)
        {
            if (obj1 is ICollection && obj2 is ICollection)
            {
                int objCollectionSize = ((ICollection)obj1).Count;
                if (objCollectionSize == ((ICollection)obj2).Count && objCollectionSize == 0)
                {
                    return false;
                }
            }
            return !obj1.Equals(obj2);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
