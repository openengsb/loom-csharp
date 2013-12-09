#region Copyright
// <copyright file="ArtifactComparingTests.cs" company="OpenEngSB">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sonatype;

namespace SonaTypeTests
{
    [TestClass]
    public class ArtifactComparingTests
    {
        #region Tests
        [TestMethod]
        public void TestIfSameArtifactreturn0WhenCompared()
        {
            Artifact ar = new Artifact();
            ar.Version = "3.0.0-Snapshot";
            Assert.AreEqual(ar.CompareTo(ar), 0);
        }
        
        [TestMethod]
        public void TestIfSameArtifactWithNullAsVersionReturn0WhenCompared()
        {
            Artifact ar = new Artifact();
            Assert.AreEqual(ar.CompareTo(ar), 0);
        }

        [TestMethod]
        public void TestIftwoArtifactWithDifferentValuesReturnMinus1WhenCompared()
        {
            Artifact ar = new Artifact();
            ar.Version = "3.0.0";
            Artifact ar2 = new Artifact();
            ar2.Version = "2.0.0";
            Assert.AreEqual(ar2.CompareTo(ar), -1);
        }
        
        [TestMethod]
        public void TestIftwoArtifactWithDifferentValuesReturn1WhenCompared()
        {
            Artifact ar = new Artifact();
            ar.Version = "3.0.0";
            Artifact ar2 = new Artifact();
            ar2.Version = "2.0.0";
            Assert.AreEqual(ar.CompareTo(ar2), 1);
        }
        
        [TestMethod]
        public void TestIftwoArtifactWhereOneVersionIsNullReturnMinus1WhenCompared()
        {
            Artifact ar = new Artifact();
            ar.Version = "3.0.0";
            Artifact ar2 = new Artifact();
            ar2.Version = null;
            Assert.AreEqual(ar.CompareTo(ar2), 1);
        }
        
        [TestMethod]
        public void TestIftwoArtifactWhereTheOthersVersionIsNullReturn1WhenCompared()
        {
            Artifact ar = new Artifact();
            ar.Version = null;
            Artifact ar2 = new Artifact();
            ar2.Version = "2.0.0";
            Assert.AreEqual(ar.CompareTo(ar2), -1);
        }
        
        [TestMethod]
        public void TestIftwoArtifactWithDifferentSpezialVersionValuesReturn1WhenCompared()
        {
            Artifact ar = new Artifact();
            ar.Version = "3.0.0-SNAPSHOT-20130705";
            Artifact ar2 = new Artifact();
            ar2.Version = "3.0.0-SNAPSHOT-20130706";
            Assert.AreEqual(ar2.CompareTo(ar), 1);
        }
        
        [TestMethod]
        public void TestIftwoArtifactWithDifferentVersionLengthValuesReturn1WhenCompared()
        {
            Artifact ar = new Artifact();
            ar.Version = "3.0.0-SNAPSHOT-20130705";
            Artifact ar2 = new Artifact();
            ar2.Version = "3.0.0-SNAPSHOT-201307057";
            Assert.AreEqual(ar2.CompareTo(ar), 1);
        }
        #endregion
    }
}