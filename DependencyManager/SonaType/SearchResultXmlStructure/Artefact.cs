using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Sonatype
{
    [Serializable]
    [XmlType("artifact")]
    public class Artifact : System.Object, IComparable<Artifact>
    {
        [XmlElement("resourceURI")]
        public String ResourceUri { get; set; }
        [XmlElement("groupId")]
        public String GroupId { get; set; }
        [XmlElement("artifactId")]
        public String ArtifactId { get; set; }
        [XmlElement("version")]
        public String Version { get; set; }
        [XmlElement("classifier")]
        public String Classifier { get; set; }
        [XmlElement("packaging")]
        public String Packaging { get; set; }
        [XmlElement("extension")]
        public String Extension { get; set; }
        [XmlElement("repoId")]
        public String RepoId { get; set; }
        [XmlElement("contextId")]
        public String ContextId { get; set; }
        [XmlElement("pomLink")]
        public String PomLink { get; set; }
        [XmlElement("artifactLink")]
        public String ArtifactLink { get; set; }

        public Artifact() { }


        public override Boolean Equals(Object artifact1)
        {
            if (!artifact1.GetType().Equals(this.GetType()))
            {
                return false;
            }
            PropertyInfo[] fields = typeof(Artifact).GetProperties();
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
            if (obj2Value != null && !obj2Value.Equals(obj1Value))
            {
                return true;
            }
            else if (obj1Value != null && !obj1Value.Equals(obj2Value))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(Artifact other)
        {
            if (String.IsNullOrEmpty(other.Version) && String.IsNullOrEmpty(this.Version))
            {
                return 0;
            }
            if (String.IsNullOrEmpty(other.Version) )
            {
                return 1;
            }
            if (String.IsNullOrEmpty(this.Version))
            {
                return -1;
            }
            String versionOther = other.Version.Replace(".","").Replace("-","").Trim();
            String thisVersion=Version.Replace(".","").Replace("-","").Trim();

            for (int i = 0; i < versionOther.Length; i++)
            {
                if (thisVersion.Length <= i)
                {
                    break;
                }
                if (!thisVersion[i].Equals(versionOther[i])){
                    return GetComparingResult(versionOther[i], thisVersion[i]);
                }
            }
            return thisVersion.Length-versionOther.Length;
        }

        private int GetComparingResult(char versionOther, char thisVersion)
        {
            int tmpResult = versionOther - thisVersion;
            if (tmpResult < 0)
            {
                return 1;
            }
            else if (tmpResult > 0)
            {
                return -1;
            }
            return 0;
        }
    }
}