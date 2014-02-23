using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sonatype.SearchResultXmlStructure;
using Sonatype;
using System.Text;
using SonaTypeDependencies;
using System.Xml.Serialization;
using System.IO;

namespace SonaTypeTests
{
    [TestClass]
    public class TestsearchResultsSerialization
    {
        [TestMethod]
        public void TestIfArtifactAreEqual()
        {
            SearchResult result = new SearchResult();
            Artifact artefact = new Artifact();
            artefact.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            artefact.GroupId = "org.openengsb.framework";
            artefact.ArtifactId = "openengsb-framework";
            artefact.Version = "2.6.0-SNAPSHOT";
            artefact.Packaging = "zip";
            artefact.Extension = "zip";
            artefact.RepoId = "ossrh-snapshot";
            artefact.ContextId = "ossrh-snapshot";
            artefact.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            result.artefacts.Add(artefact);
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
            Artifact artefact = new Artifact();
            artefact.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            artefact.GroupId = "org.openengsb.framework";
            artefact.ArtifactId = "openengsb-framework";
            artefact.Version = "2.6.0-SNAPSHOT";
            artefact.Packaging = "zip";
            artefact.Extension = "zip";
            artefact.RepoId = "ossrh-snapshot";
            artefact.ContextId = "ossrh-snapshot";
            artefact.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            result.artefacts.Add(artefact);
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
            Artifact artefact = new Artifact();
            artefact.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            artefact.GroupId = "org.openengsb.framework";
            artefact.ArtifactId = "openengsb-framework";
            artefact.Version = "2.6.0-SNAPSHOT";
            artefact.Packaging = "zip";
            artefact.Extension = "zip";
            artefact.RepoId = "ossrh-snapshot";
            artefact.ContextId = "ossrh-snapshot";
            artefact.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            result.artefacts.Add(artefact);
            result.Count = 1;
            result.From = "test";
            result.TooManyResults = false;
            result.TotalCount = 1;

            SearchResult resultNotEqual = new SearchResult();
            Artifact artefactNotEqual = new Artifact();
            artefactNotEqual.ResourceUri = "https://repository.sonatype.org/service/local/repositories/ossrh-snapshot/content/org/openengsb/framework/openengsb-framework/2.6.0-SNAPSHOT/openengsb-framework-2.6.0-SNAPSHOT-src.zip";
            artefactNotEqual.GroupId = "org.openengsb.framework";
            artefactNotEqual.ArtifactId = "openengsb-framework";
            artefactNotEqual.Version = "2.6.0-SNAPSHOT";
            artefactNotEqual.Packaging = "zip";
            artefactNotEqual.Extension = "zip";
            artefactNotEqual.RepoId = "ossrh-snapshot";
            artefactNotEqual.ContextId = "ossrh-snapshot";
            artefactNotEqual.ArtifactLink = "https://repository.sonatype.org/service/local/artifact/maven/redirect?r=ossrh-snapshot&amp;g=org.openengsb.framework&amp;a=openengsb-framework&amp;v=2.6.0-SNAPSHOT&amp;e=zip&amp;c=src";
            artefactNotEqual.PomLink = "DIFFRENT TO ARTEFACT1";
            resultNotEqual.artefacts.Add(artefactNotEqual);
            resultNotEqual.Count = 1;
            resultNotEqual.From = "test";
            resultNotEqual.TooManyResults = false;
            resultNotEqual.TotalCount = 1;

            Assert.AreNotEqual<SearchResult>(result, resultNotEqual);
        }
        [TestMethod]
        public void TestSerializer()
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
            expectedSearchResult.artefacts.Add(expectedResult);
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

            SonatypeDependencyManager sonaType = new SonatypeDependencyManager(null,null,null,null,null);
            SearchResult resultArtefact = sonaType.ConvertSearchResult<SearchResult>(builder.ToString());
            Assert.AreNotEqual<SearchResult>(expectedSearchResult, resultArtefact);

        }
        [TestMethod]
        public void TestSerializerWithTwoArtefactEntries()
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
            expectedSearchResult.artefacts.Add(expectedResult);
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
            expectedSearchResult.artefacts.Add(expectedResult);

            expectedSearchResult.artefacts.Add(expectedResult);
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
            SearchResult resultArtefact = sonaType.ConvertSearchResult<SearchResult>(builder.ToString());
            Assert.AreNotEqual<SearchResult>(expectedSearchResult, resultArtefact);

        }

    }
}
