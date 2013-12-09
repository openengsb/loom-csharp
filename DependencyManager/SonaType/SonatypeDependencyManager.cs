#region Copyright
// <copyright file="SonatypeDependencyManager.cs" company="OpenEngSB">
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
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using log4net;
using Sonatype;
using Sonatype.SearchResultXmlStructure;
using ZetaLongPaths;

namespace SonaTypeDependencies
{
    public class SonatypeDependencyManager
    {
        #region Constants
        private const String BaseUrl = "http://repository.sonatype.org/";
        private const String RestFullBaseUrl = "https://oss.sonatype.org/content/repositories/snapshots";
        private const String SearchUrl = "service/local/data_index?";
        private const String GroupSearchParameter = "g={0}";
        private const String ArtifactSearchParameter = "&a={0}";
        private const String PackageSearchParameter = "&p={0}";
        private const String ExtensionSearchParameter = "&e={0}";
        private const String VersionSearchParameter = "&v={0}";
        #endregion
        #region Logger
        private static ILog logger = LogManager.GetLogger(typeof(SonatypeDependencyManager));
        #endregion
        #region Private Variables
        private String groupId;
        private String artefactId;
        private String version;
        private String packaging;
        private String classifier;
        private WebClient client = new WebClient();
        #endregion
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="groupId">Group ID</param>
        /// <param name="artefactId">Artefact ID</param>
        /// <param name="version">Version</param>
        /// <param name="packaging">Packaging</param>
        /// <param name="classifier">Classifier</param>
        public SonatypeDependencyManager(String groupId, String artefactId, String version, String packaging, String classifier)
        {
            this.groupId = groupId;
            this.artefactId = artefactId;
            this.version = version;
            this.packaging = packaging;
            this.classifier = classifier;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Downloads the Artifact according to the parameters that has been
        /// indicated over the constructor
        /// </summary>
        /// <param name="folderLocation">The location where the Artefact should be dowloaded</param>
        /// <returns>The Absolute Path to the Artefact</returns>
        public FileInfo DownloadArtifactToFolder(String folderLocation)
        {
            logger.Info("Download all the Artefacts (In XML format)");
            String searchResult = this.client.DownloadString(new Uri(this.GetSearchUrl()));
            logger.Info("Convert the XML to SearchResult object");
            SearchResult result = ConvertSearchResult<SearchResult>(searchResult);
            logger.Info("Add the Artefacts by search the artefacts (Because of a Bug in Nexus sonatype)");
            result.Artifacts.AddRange(this.FindArtifactOverRest());
            logger.Info("Search for the artefact that fulfils all the criteria");
            Artifact selectedArtifact = this.FindCorrectArtifact(result);
            String folderLocationAndFileName = Path.Combine(folderLocation, selectedArtifact.ArtifactId + "-" + selectedArtifact.Version + "." + selectedArtifact.Packaging);
            logger.Info("Download the Artefact to the folder " + folderLocationAndFileName);
            this.client.DownloadFile(selectedArtifact.ArtifactLink, folderLocationAndFileName);
            return new FileInfo(folderLocationAndFileName);
        }

        /// <summary>
        /// Converts the SearchResult (in XML) to an Object
        /// </summary>
        /// <typeparam name="ResultType"></typeparam>
        /// <param name="searchResult"></param>
        /// <returns></returns>
        public ResultType ConvertSearchResult<ResultType>(String searchResult)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ResultType));
            StringReader reader = new StringReader(searchResult);
            return (ResultType)serializer.Deserialize(reader);
        }
        #endregion
        #region Private Methods
        private Artifact FindCorrectArtifact(SearchResult result)
        {
            List<Artifact> matchingArtefacts = this.RemoveWrongArtifacts(result.Artifacts);
            if (matchingArtefacts.Count > 0)
            {
                Artifact oldestArtefact = null;
                foreach (Artifact art in matchingArtefacts)
                {
                    if (oldestArtefact == null)
                    {
                        oldestArtefact = art;
                        continue;
                    }

                    if (oldestArtefact.CompareTo(art) > 0)
                    {
                        oldestArtefact = art;
                    }
                }

                return oldestArtefact;
            }

            throw new ArgumentException("For the given parameters, no Artefact could be found");
        }

        private void CreateAllNotExistingFolder(ZlpDirectoryInfo directories)
        {
            if (directories.Exists)
            {
                return;
            }

            this.CreateAllNotExistingFolder(directories.Parent);
            Microsoft.Experimental.IO.LongPathDirectory.Create(directories.FullName);
        }

        private bool IsStringNotEmpty(String value)
        {
            return !String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Creates a Url from the groupId, artefactId and the Version
        /// </summary>
        /// <returns></returns>
        private String GetSearchUrl()
        {
            logger.Info("Generate Search Url");
            if (String.IsNullOrEmpty(this.groupId) || String.IsNullOrEmpty(this.artefactId)
                || String.IsNullOrEmpty(this.packaging))
            {
                throw new ArgumentException("The packageName, version or packaging can not be empty");
            }

            StringBuilder builder = new StringBuilder(BaseUrl);
            builder.Append(SearchUrl);
            builder.Append(String.Format(GroupSearchParameter, this.groupId));
            builder.Append(String.Format(ArtifactSearchParameter, this.artefactId));
            if (this.IsStringNotEmpty(this.version))
            {
                builder.Append(String.Format(VersionSearchParameter, this.version));
            }

            builder.Append(String.Format(PackageSearchParameter, this.packaging));
            return builder.ToString();
        }

        /// <summary>
        /// Nexus sonatype does not list elements that have no classifier.
        /// To find zip files (wiht no classifier) this workaround is used.
        /// (Search the file over a Rest url)
        /// </summary>
        /// <returns></returns>
        private List<Artifact> FindArtifactOverRest()
        {
            StringBuilder urlBuilder = new StringBuilder(RestFullBaseUrl);
            foreach (String element in this.groupId.Split('.'))
            {
                urlBuilder.Append("/");
                urlBuilder.Append(element);
            }

            urlBuilder.Append("/");
            urlBuilder.Append(this.artefactId);
            urlBuilder.Append("/");
            urlBuilder.Append(this.version);
            urlBuilder.Append("/");
            return HtmlArtifact.ConvertToArtifacts(this.client.DownloadString(urlBuilder.ToString()));
        }

        /// <summary>
        /// Returns all the artefacts that fullfils the parameters (indicated over the constructor)
        /// </summary>
        /// <param name="artefacts"></param>
        /// <returns></returns>
        private List<Artifact> RemoveWrongArtifacts(List<Artifact> artefacts)
        {
            return artefacts.FindAll(ar =>
            {
                return ar.ArtifactId == this.artefactId
                    && ar.Classifier == this.classifier
                    && ar.Packaging == this.packaging;
            });
        }
        #endregion
    }
}