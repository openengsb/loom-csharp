using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sonatype.SearchResultXmlStructure
{
    public enum Classifier
    {
        src,pom, zip
    }
    public class HtmlArtifact : Artifact
    {
        private const String LinkEndTak = "</a>";
        private const String HrefStart = "<a href=\"";
        public HtmlArtifact() { }
        public HtmlArtifact(String href)
        {
            int start = href.IndexOf("=") + 1;
            int end = href.IndexOf(">");
            String url = href.Substring(start, end - start);
            String tmpname = href.Substring(end + 1);
            this.ArtifactLink = url.Replace("\"", "");
            String name = tmpname.Substring(0, tmpname.IndexOf("<"));
            int firstMinusPosition = name.IndexOf("-") + 1;
            if (firstMinusPosition <= 0)
            {
                this.ArtifactId = name;
                return;
            }
            this.Packaging = name.Substring(name.LastIndexOf(".") + 1);
            StringBuilder builder = new StringBuilder();
            foreach (String element in name.Split('-'))
            {
                if (notStartsWithDigit(element))
                {
                    this.ArtifactId += element + "-";
                }
                else
                {
                    this.ArtifactId = this.ArtifactId.Substring(0, this.ArtifactId.Length - 1);
                    break;
                }
            }
            this.Version = name.Replace(this.ArtifactId, "").Replace(this.Packaging, "");
            this.Version = Version.Substring(1,this.Version.Length-2);
            foreach (Classifier classifier in Enum.GetValues(typeof(Classifier)))
            {
                if (Version.Contains(classifier.ToString()))
                {
                    this.Classifier = classifier.ToString();
                    this.Version.Replace(this.Classifier, "");
                    break;
                }
            }
        }
        public Boolean notStartsWithDigit(string value)
        {
            if (value.Length<=0){
                return true;
            }
            String possibleDigit = value[0]+"";
            try
            {
                int.Parse(possibleDigit);
                return false;
            }
            catch
            {
            }
            return true;
        }
        public static List<Artifact> getinstance(String htmlpage)
        {
            List<Artifact> result = new List<Artifact>();
            String intermediatResult = "";
            HtmlArtifact sa = new HtmlArtifact();
            String tmp = htmlpage;
            while (tmp.Contains(HrefStart))
            {
                int end = tmp.IndexOf(LinkEndTak) + LinkEndTak.Length;
                int start = tmp.IndexOf(HrefStart);
                intermediatResult = tmp.Substring(start, end - start);
                tmp = tmp.Substring(end);
                result.Add(new HtmlArtifact(intermediatResult));
            }
            return result;
        }
    }
}