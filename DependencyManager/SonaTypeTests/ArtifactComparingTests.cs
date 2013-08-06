using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sonatype;

namespace SonaTypeTests
{
    [TestClass]
    public class ArtifactComparingTests
    {
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
    }
}