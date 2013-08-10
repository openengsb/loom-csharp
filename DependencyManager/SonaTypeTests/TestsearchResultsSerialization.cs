#region Copyright
// <copyright file="TestsearchResultsSerialization.cs" company="OpenEngSB">
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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sonatype;
using Sonatype.SearchResultXmlStructure;
using SonaTypeDependencies;

namespace SonaTypeTests
{
    [TestClass]
    public class TestsearchResultsSerialization
    {
        #region Tests
        [TestMethod]
        public void TestIfArtifactAreEqual()
        {
            SearchResult result = new SearchResult();
            Artifact artifact = new Artifact();
            artifact.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            artifact.GroupId = "org.openengsb.framework";
            artifact.ArtifactId = "openengsb-framework";
            artifact.Version = "2.6.0-SNAPSHOT";
            artifact.Packaging = "zip";
            artifact.Extension = "zip";
            artifact.RepoId = "ossrh-snapshot";
            artifact.ContextId = "ossrh-snapshot";
            artifact.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            result.Artifacts.Add(artifact);
            result.Count = 1;
            result.From = "test";
            result.TooManyResults = false;
            result.TotalCount = 1;
            Assert.AreEqual<SearchResult>(result, result);
        }

        [TestMethod]
        public void TestIfArtifactAreNotEqualWhereOneObjectIsEmpty()
        {
            SearchResult result = new SearchResult();
            Artifact artifact = new Artifact();
            artifact.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            artifact.GroupId = "org.openengsb.framework";
            artifact.ArtifactId = "openengsb-framework";
            artifact.Version = "2.6.0-SNAPSHOT";
            artifact.Packaging = "zip";
            artifact.Extension = "zip";
            artifact.RepoId = "ossrh-snapshot";
            artifact.ContextId = "ossrh-snapshot";
            artifact.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            result.Artifacts.Add(artifact);
            result.Count = 1;
            result.From = "test";
            result.TooManyResults = false;
            result.TotalCount = 1;
            Assert.AreNotEqual<SearchResult>(result, new SearchResult());
        }

        [TestMethod]
        public void TestIfArtifactAreEqualWithEmptyArifacts()
        {
            Assert.AreEqual<SearchResult>(new SearchResult(), new SearchResult());
        }
        
        [TestMethod]
        public void TestArtifactIsNotEqual()
        {
            SearchResult result = new SearchResult();
            Artifact artifact = new Artifact();
            artifact.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            artifact.GroupId = "org.openengsb.framework";
            artifact.ArtifactId = "openengsb-framework";
            artifact.Version = "2.6.0-SNAPSHOT";
            artifact.Packaging = "zip";
            artifact.Extension = "zip";
            artifact.RepoId = "ossrh-snapshot";
            artifact.ContextId = "ossrh-snapshot";
            artifact.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            result.Artifacts.Add(artifact);
            result.Count = 1;
            result.From = "test";
            result.TooManyResults = false;
            result.TotalCount = 1;

            SearchResult resultNotEqual = new SearchResult();
            Artifact artifactNotEqual = new Artifact();
            artifactNotEqual.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            artifactNotEqual.GroupId = "org.openengsb.framework";
            artifactNotEqual.ArtifactId = "openengsb-framework";
            artifactNotEqual.Version = "2.6.0-SNAPSHOT";
            artifactNotEqual.Packaging = "zip";
            artifactNotEqual.Extension = "zip";
            artifactNotEqual.RepoId = "ossrh-snapshot";
            artifactNotEqual.ContextId = "ossrh-snapshot";
            artifactNotEqual.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            artifactNotEqual.PomLink = "DIFFRENT TO ARTIFACT1";
            resultNotEqual.Artifacts.Add(artifactNotEqual);
            resultNotEqual.Count = 1;
            resultNotEqual.From = "test";
            resultNotEqual.TooManyResults = false;
            resultNotEqual.TotalCount = 1;

            Assert.AreNotEqual<SearchResult>(result, resultNotEqual);
        }
        
        [TestMethod]
        public void TestIfSerializerCreatesTheCorrectResult()
        {
            SearchResult expectedSearchResult = new SearchResult();
            Artifact expectedResult = new Artifact();
            expectedResult.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            expectedResult.GroupId = "org.openengsb.framework";
            expectedResult.ArtifactId = "openengsb-framework";
            expectedResult.Version = "2.6.0-SNAPSHOT";
            expectedResult.Packaging = "zip";
            expectedResult.Extension = "zip";
            expectedResult.RepoId = "ossrh-snapshot";
            expectedResult.ContextId = "ossrh-snapshot";
            expectedResult.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            expectedSearchResult.Artifacts.Add(expectedResult);
            expectedSearchResult.Count = -1;
            expectedSearchResult.From = "-1";
            expectedSearchResult.TooManyResults = false;
            expectedSearchResult.TotalCount = 28;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<search-results>");
            builder.AppendLine("<totalCount>28</totalCount>");
            builder.AppendLine("<from>-1</from>");
            builder.AppendLine("<count>-1</count>");
            builder.AppendLine("<tooManyResults>false</tooManyResults>");
            builder.AppendLine("<data>");
            builder.AppendLine("<artifact>");
            builder.AppendLine("<resourceURI>https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip</resourceURI>");
            builder.AppendLine("<groupId>org.openengsb.framework</groupId>");
            builder.AppendLine("<artifactId>openengsb-framework</artifactId>");
            builder.AppendLine("<version>2.6.0-SNAPSHOT</version>");
            builder.AppendLine("<classifier>src</classifier>");
            builder.AppendLine("<packaging>zip</packaging>");
            builder.AppendLine("<extension>zip</extension>");
            builder.AppendLine("<repoId>ossrh-snapshot</repoId>");
            builder.AppendLine("<contextId>ossrh-snapshot</contextId>");
            builder.AppendLine("<pomLink></pomLink>");
            builder.AppendLine("<artifactLink>https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src</artifactLink>");
            builder.AppendLine("</artifact>");
            builder.AppendLine("</data>");
            builder.AppendLine("</search-results>");

            SonatypeDependencyManager sonaType = new SonatypeDependencyManager(null, null, null, null, null);
            SearchResult resultArtifact = sonaType.ConvertSearchResult<SearchResult>(builder.ToString());
            Assert.AreNotEqual<SearchResult>(expectedSearchResult, resultArtifact);
        }
        
        [TestMethod]
        public void TestSerializerWithTwoArtifactEntries()
        {
            SearchResult expectedSearchResult = new SearchResult();
            Artifact expectedResult = new Artifact();
            expectedResult.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            expectedResult.GroupId = "org.openengsb.framework";
            expectedResult.ArtifactId = "openengsb-framework";
            expectedResult.Version = "2.6.0-SNAPSHOT";
            expectedResult.Packaging = "zip";
            expectedResult.Extension = "zip";
            expectedResult.RepoId = "ossrh-snapshot";
            expectedResult.ContextId = "ossrh-snapshot";
            expectedResult.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            expectedSearchResult.Artifacts.Add(expectedResult);
            expectedResult = new Artifact();
            expectedResult.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            expectedResult.GroupId = "org.openengsb.framework";
            expectedResult.ArtifactId = "openengsb-framework";
            expectedResult.Version = "2.4.1";
            expectedResult.Packaging = "zip";
            expectedResult.Extension = "zip";
            expectedResult.RepoId = "central-proxy";
            expectedResult.ContextId = "Central Proxy";
            expectedResult.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            expectedSearchResult.Artifacts.Add(expectedResult);

            expectedSearchResult.Artifacts.Add(expectedResult);
            expectedSearchResult.Count = -1;
            expectedSearchResult.From = "-1";
            expectedSearchResult.TooManyResults = false;
            expectedSearchResult.TotalCount = 28;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<search-results>");
            builder.AppendLine("<totalCount>28</totalCount>");
            builder.AppendLine("<from>-1</from>");
            builder.AppendLine("<count>-1</count>");
            builder.AppendLine("<tooManyResults>false</tooManyResults>");
            builder.AppendLine("<data>");
            builder.AppendLine("<artifact>");
            builder.AppendLine("<resourceURI>https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip</resourceURI>");
            builder.AppendLine("<groupId>org.openengsb.framework</groupId>");
            builder.AppendLine("<artifactId>openengsb-framework</artifactId>");
            builder.AppendLine("<version>2.6.0-SNAPSHOT</version>");
            builder.AppendLine("<classifier>src</classifier>");
            builder.AppendLine("<packaging>zip</packaging>");
            builder.AppendLine("<extension>zip</extension>");
            builder.AppendLine("<repoId>ossrh-snapshot</repoId>");
            builder.AppendLine("<contextId>ossrh-snapshot</contextId>");
            builder.AppendLine("<pomLink></pomLink>");
            builder.AppendLine("<artifactLink>https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src</artifactLink>");
            builder.AppendLine("</artifact>");
            builder.AppendLine("<artifact>");
            builder.AppendLine("<resourceURI>https://repository.sonatype.org/service/local/repositories/central-proxy/content/org/openengsb/framework/openengsb-framework/2.4.1/openengsb-framework-2.4.1-src.zip</resourceURI>");
            builder.AppendLine("<groupId>org.openengsb.framework</groupId>");
            builder.AppendLine("<artifactId>openengsb-framework</artifactId>");
            builder.AppendLine("<version>2.4.1</version>");
            builder.AppendLine("<classifier>src</classifier>");
            builder.AppendLine("<packaging>zip</packaging>");
            builder.AppendLine("<extension>zip</extension>");
            builder.AppendLine("<repoId>central-proxy</repoId>");
            builder.AppendLine("<contextId>Central Proxy</contextId>");
            builder.AppendLine("<pomLink></pomLink>");
            builder.AppendLine("<artifactLink>https://repository.sonatype.org/service/local/artifact/maven/redirect?r=central-proxy&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.4.1&amp;e=zip&amp;c=src</artifactLink>");
            builder.AppendLine("</artifact>");
            builder.AppendLine("</data>");
            builder.AppendLine("</search-results>");

            SonatypeDependencyManager sonaType = new SonatypeDependencyManager(null, null, null, null, null);
            SearchResult resultArtifact = sonaType.ConvertSearchResult<SearchResult>(builder.ToString());
            Assert.AreNotEqual<SearchResult>(expectedSearchResult, resultArtifact);
        }
        #endregion
    }
}