#region Copyright
// <copyright file="ArtifactSerializerTests.cs" company="OpenEngSB">
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
using SonaTypeDependencies;

namespace SonaTypeTests
{
    [TestClass]
    public class ArtifactSerializerTests
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

            SonatypeDependencyManager sonaType = new SonatypeDependencyManager(null, null, null, null, null);
            Artifact resultArtefact = sonaType.ConvertSearchResult<Artifact>(builder.ToString());
            Assert.AreNotEqual<Artifact>(expectedResult, resultArtefact);
        }
    }
}