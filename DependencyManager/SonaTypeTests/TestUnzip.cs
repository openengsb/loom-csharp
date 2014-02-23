#region Copyright
// <copyright file="TestUnzip.cs" company="OpenEngSB">
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
using Microsoft.Experimental.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonaTypeDependencies;
using UnzipArtifact;

namespace SonaTypeTests
{
    [TestClass]
    public class TestUnzip
    {
        #region Private Variables
        private DirectoryInfo directory = null;
        private DirectoryInfo tempFolder;
        private IUnzipper unzipper;
        #endregion
        #region Initializer
        [TestInitialize]
        public void InitTests()
        {
            this.tempFolder = new DirectoryInfo(System.IO.Path.GetTempFileName() + Guid.NewGuid());
            this.tempFolder.Create();
        }
        #endregion
        #region Tests
        [TestMethod]
        public void TestUnzipTestWithOneFolderAndOneFileFolderExists()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWithOneFolderAndOneFile.7zip");
            unzipper = new SevenZipUnzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            Assert.IsTrue(this.directory.Exists);
        }

        [TestMethod]
        public void TestUnzipTestWithOneFolderAndOneFileFileExists()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWithOneFolderAndOneFile.7zip");
            unzipper = new SevenZipUnzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            FileInfo testFile = new FileInfo(this.directory.FullName + "/" + "New Text Document.txt");
            Assert.IsTrue(testFile.Exists);
        }

        [TestMethod]
        public void TestIfAllFoldersArePresentWhenUnzipTestWith2LevelsOfFolders()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWith2LevelsOfFolders.7zip");
            unzipper = new SevenZipUnzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            Assert.AreEqual<int>(this.directory.GetDirectories().Length, 32);
        }

        [TestMethod]
        public void TestIfAllFilesArePresentWhenUnzipTestWith2LevelsOfFolders()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWith2LevelsOfFolders.7zip");
            unzipper = new SevenZipUnzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            Assert.AreEqual<int>(this.directory.GetFiles().Length, 5);
        }

        [TestMethod]
        public void TestIfAllSubFilesArePresentWhenUnzipTestWith2LevelsOfFolders()
        {
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWith2LevelsOfFolders.7zip");
            unzipper = new SevenZipUnzipper(zipFile);
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
            // Does not work with 7zip. In a futer Version, a unzipper is added that allows it to unpack Long pathes.
            FileInfo zipFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWithPathToLongInside.7zip");
            unzipper = new SevenZipUnzipper(zipFile);
            String folder = unzipper.UnzipFile(tempFolder.FullName);
            this.directory = new DirectoryInfo(folder);
            Assert.AreEqual<int>(this.GetLongestPathDirectory(this.directory).FullName.Length, 248);
        }
        #endregion
        #region Cleanup
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
        #endregion
        #region Private Methods
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
        #endregion
    }
}