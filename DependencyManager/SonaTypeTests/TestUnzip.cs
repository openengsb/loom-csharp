using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Experimental.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonaTypeDependencies;
using UnzipArtifact;

namespace SonaTypeTests
{
    [TestClass]
    public class TestUnzip
    {
        private DirectoryInfo directory = null;
        private DirectoryInfo tempFolder;
        private Unzipper unzipper;
        [TestInitialize]
        public void InitTests()
        {
            this.tempFolder = new DirectoryInfo(System.IO.Path.GetTempFileName() + Guid.NewGuid());
            this.tempFolder.Create();
        }

        [TestMethod]
        public void TestUnzipTestWithOneFolderAndOneFileFolderExists()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWithOneFolderAndOneFile.zip");
            unzipper = new Unzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            Assert.IsTrue(this.directory.Exists);
        }

        [TestMethod]
        public void TestUnzipTestWithOneFolderAndOneFileFileExists()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWithOneFolderAndOneFile.zip");
            unzipper = new Unzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            FileInfo testFile = new FileInfo(this.directory.FullName + "/" + "New Text Document.txt");
            Assert.IsTrue(testFile.Exists);
        }
        
        [TestMethod]
        public void TestIfAllFoldersArePresentWhenUnzipTestWith2LevelsOfFolders()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWith2LevelsOfFolders.zip");
            unzipper = new Unzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            Assert.AreEqual<int>(this.directory.GetDirectories().Length, 32);
        }
        
        [TestMethod]
        public void TestIfAllFilesArePresentWhenUnzipTestWith2LevelsOfFolders()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWith2LevelsOfFolders.zip");
            unzipper = new Unzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            Assert.AreEqual<int>(this.directory.GetFiles().Length, 5);
        }
        
        [TestMethod]
        public void TestIfAllSubFilesArePresentWhenUnzipTestWith2LevelsOfFolders()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWith2LevelsOfFolders.zip");
            unzipper = new Unzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            List<FileInfo> files = new List<FileInfo>();
            foreach (DirectoryInfo directoryInfo in this.directory.GetDirectories())
            {
                files.AddRange(directoryInfo.GetFiles());
            }

            Assert.AreEqual<int>(files.Count, 2);
        }
        
        [Ignore]
        public void TestUnzipTestWithPathToLong()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWithPathToLongInside.zip");
            unzipper = new Unzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            Assert.AreEqual<int>(this.GetLongestPathDirectory(this.directory).FullName.Length, 248);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (this.directory != null)
            {
                try
                {
                    this.directory.Delete(true);
                }
                catch
                {
                    // C# Framework library can not handle files that are to long
                    this.DeleteAllFolder(this.directory.FullName);
                }
            }

            this.tempFolder.Delete(true);
        }

        private DirectoryInfo GetLongestPathDirectory(DirectoryInfo directory)
        {
            if (directory == null)
            {
                return null;
            }
            
            int max = directory.FullName.Length;
            DirectoryInfo result = directory;
            foreach (DirectoryInfo d in this.directory.GetDirectories())
            {
                if (d.FullName.Length > max)
                {
                    max = d.FullName.Length;
                    result = d;
                }

                DirectoryInfo tmp = this.GetLongestPathDirectory(d);
                if (tmp != null && tmp.FullName.Length > max)
                {
                    result = tmp;
                    max = tmp.FullName.Length;
                }
            }
            
            return result;
        }

        private void DeleteAllFolder(String parentDirectory)
        {
            if (parentDirectory == null)
            {
                return;
            }
            
            foreach (String file in LongPathDirectory.EnumerateFiles(parentDirectory))
            {
                LongPathFile.Delete(file);
            }
            
            foreach (String info in LongPathDirectory.EnumerateDirectories(parentDirectory))
            {
                this.DeleteAllFolder(info);
            }
            
            Microsoft.Experimental.IO.LongPathDirectory.Delete(parentDirectory);
        }
    }
}