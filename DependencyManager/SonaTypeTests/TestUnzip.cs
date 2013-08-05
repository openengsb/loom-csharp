using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonaTypeDependencies;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Experimental.IO;

namespace SonaTypeTests
{
    [TestClass]
    public class TestUnzip
    {
        private DirectoryInfo directory = null;
        private DirectoryInfo tempFolder;
        [TestInitialize]
        public void InitTests()
        {
            tempFolder= new DirectoryInfo(System.IO.Path.GetTempFileName()+Guid.NewGuid());
            tempFolder.Create();
        }
        [TestMethod]
        public void TestUnzipTestWithOneFolderAndOneFileFolderExists()
        {
            FileInfo d = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWithOneFolderAndOneFile.zip");
            SonatypeDependencyManager dmanager = new SonatypeDependencyManager(null, null, null, null, null);
            String folder = dmanager.UnzipFile(d.FullName,tempFolder.FullName);
            directory = new DirectoryInfo(folder);
            Assert.IsTrue(directory.Exists);
        }
        [TestMethod]
        public void TestUnzipTestWithOneFolderAndOneFileFileExists()
        {
            FileInfo d = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWithOneFolderAndOneFile.zip");
            SonatypeDependencyManager dmanager = new SonatypeDependencyManager(null, null, null, null, null);
            String folder = dmanager.UnzipFile(d.FullName, tempFolder.FullName);
            directory = new DirectoryInfo(folder);
            FileInfo testFile = new FileInfo(directory.FullName + "/" + "New Text Document.txt");
            Assert.IsTrue(testFile.Exists);
        }
        [TestMethod]
        public void TestIfAllFoldersArePresentWhenUnzipTestWith2LevelsOfFolders()
        {
            FileInfo d = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWith2LevelsOfFolders.zip");
            SonatypeDependencyManager dmanager = new SonatypeDependencyManager(null, null, null, null, null);
            String folder = dmanager.UnzipFile(d.FullName, tempFolder.FullName);
            directory = new DirectoryInfo(folder);
            Assert.AreEqual<int>(directory.GetDirectories().Length, 32);
        }
        [TestMethod]
        public void TestIfAllFilesArePresentWhenUnzipTestWith2LevelsOfFolders()
        {
            FileInfo d = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWith2LevelsOfFolders.zip");
            SonatypeDependencyManager dmanager = new SonatypeDependencyManager(null, null, null, null, null);
            String folder = dmanager.UnzipFile(d.FullName, tempFolder.FullName);
            directory = new DirectoryInfo(folder);
            Assert.AreEqual<int>(directory.GetFiles().Length, 5);
        }
        [TestMethod]
        public void TestIfAllSubFilesArePresentWhenUnzipTestWith2LevelsOfFolders()
        {
            FileInfo d = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWith2LevelsOfFolders.zip");
            SonatypeDependencyManager dmanager = new SonatypeDependencyManager(null, null, null, null, null);
            String folder = dmanager.UnzipFile(d.FullName, tempFolder.FullName);
            directory = new DirectoryInfo(folder);
            List<FileInfo> files = new List<FileInfo>();
            foreach (DirectoryInfo directoryInfo in directory.GetDirectories())
            {
                files.AddRange(directoryInfo.GetFiles());
            }
            Assert.AreEqual<int>(files.Count, 2);
        }
        [Ignore]
        public void TestUnzipTestWithPathToLong()
        {
            FileInfo d = new FileInfo(Directory.GetCurrentDirectory() + @"\Resources\TestWithPathToLongInside.zip");
            SonatypeDependencyManager dmanager = new SonatypeDependencyManager(null, null, null, null, null);
            String folder = dmanager.UnzipFile(d.FullName, tempFolder.FullName);
            directory = new DirectoryInfo(folder);
            Assert.AreEqual<int>(getLongestPathDirectory(directory).FullName.Length, 248);
        }
        private DirectoryInfo getLongestPathDirectory(DirectoryInfo directory)
        {
            if (directory == null)
            {
                return null;
            }
            int max = directory.FullName.Length;
            DirectoryInfo result = directory;
            foreach (DirectoryInfo d in directory.GetDirectories())
            {
                if (d.FullName.Length > max)
                {
                    max = d.FullName.Length;
                    result = d;
                }
                DirectoryInfo tmp = getLongestPathDirectory(d);
                if (tmp != null && tmp.FullName.Length > max)
                {
                    result = tmp;
                    max = tmp.FullName.Length;
                }
            }
            return result;
        }
        private void deleteAllFolder(String parentDirectory)
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
                deleteAllFolder(info);
            }
            Microsoft.Experimental.IO.LongPathDirectory.Delete(parentDirectory);
        }
        [TestCleanup]
        public void cleanup()
        {
            if (directory != null)
            {
                try
                {
                    directory.Delete(true);
                }
                catch
                {
                    //C# Framework library can not handle files that are to long
                    deleteAllFolder(directory.FullName);
                }
            }
            tempFolder.Delete(true);
        }
    }
}