#region Copyright
// <copyright file="HtmlArtifact.cs" company="OpenEngSB">
// Licensed to the Austrian Association for Software Tool Integration (AASTI)
// under one or more contributor license agreements. See the NOTICE file
// distributed with this work for additional information regarding copyright
// ownership. The AASTI licenses this file to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sonatype.SearchResultXmlStructure
{
    public enum Classifier
    {
        src, pom, zip
    }

    public class HtmlArtifact : Artifact
    {
        private const String LinkEndTak = "</a>";
        private const String HrefStart = "<a href=\"";
        
        public HtmlArtifact() 
        { 
        }

        public HtmlArtifact(String href)
        {
            int start = href.IndexOf("=") + 1;
            int end = href.IndexOf(">");
            String url = href.Substring(start, end - start);
            String tmpname = href.Substring(end + 1);
            this.ArtifactLink = url.Replace("\"", String.Empty);
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
                if (NotStartsWithDigit(element))
                {
                    this.ArtifactId += element + "-";
                }
                else
                {
                    this.ArtifactId = this.ArtifactId.Substring(0, this.ArtifactId.Length - 1);
                    break;
                }
            }
        
            this.Version = name.Replace(this.ArtifactId, String.Empty).Replace(this.Packaging, String.Empty);
            this.Version = Version.Substring(1, this.Version.Length - 2);
            foreach (Classifier classifier in Enum.GetValues(typeof(Classifier)))
            {
                if (Version.Contains(classifier.ToString()))
                {
                    this.Classifier = classifier.ToString();
                    this.Version.Replace(this.Classifier, String.Empty);
                    break;
                }
            }
        }

        public static List<Artifact> ConvertToArtifacts(String htmlpage)
        {
            List<Artifact> result = new List<Artifact>();
            String intermediatResult = String.Empty;
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

        private bool NotStartsWithDigit(string value)
        {
            if (value.Length <= 0)
            {
                return true;
            }

            String possibleDigit = value[0] + String.Empty;
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
    }
}