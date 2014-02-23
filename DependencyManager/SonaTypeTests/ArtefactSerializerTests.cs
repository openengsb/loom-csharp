using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Sonatype;
using SonaTypeDependencies;
using System.Xml;

namespace SonaTypeTests
{
    [TestClass]
    public class ArtefactSerializerTests
    {
        [TestMethod]
        public void TestIfArtifactAreEqual()
        {
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

            Assert.AreEqual<Artifact>(artefact, artefact);
        }
        [TestMethod]
        public void TestIfArtifactAreNotEqualWhereOneObjectIsEmpty()
        {
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

            Assert.AreNotEqual<Artifact>(artefact, new Artifact());
        }
        [TestMethod]
        public void TestIfArtifactAreEqualWithEmptyArifacts()
        {
            Assert.AreEqual<Artifact>(new Artifact(), new Artifact());
        }
        [TestMethod]
        public void TestArtifactIsNotEqual()
        {
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
            Assert.AreNotEqual<Artifact>(artefact, artefactNotEqual);
        }
        [TestMethod]
        public void TestSerializerCreatesTheCorrectOutput()
        {
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

            StringBuilder builder = new StringBuilder();
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

            SonatypeDependencyManager sonaType = new SonatypeDependencyManager(null,null,null,null,null);
            Artifact resultArtefact = sonaType.ConvertSearchResult<Artifact>(builder.ToString());
            Assert.AreNotEqual<Artifact>(expectedResult, resultArtefact);
        }

    }
}
