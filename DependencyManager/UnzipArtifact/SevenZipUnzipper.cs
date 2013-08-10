#region Copyright
// <copyright file="SevenZipUnzipper.cs" company="OpenEngSB">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace UnzipArtifact
{
    public class SevenZipUnzipper : IUnzipper
    {
        private const String SevenZipFolderLocation = @"\7-zip\7z.dll";
        private static ILog logger = LogManager.GetLogger(typeof(SevenZipUnzipper));
        private FileInfo zipFileLocation;

        public SevenZipUnzipper(FileInfo zipFileLocation)
        {
            this.zipFileLocation = zipFileLocation;
        }

        /// <summary>
        /// Unzip a Zip file to a defined folder
        /// </summary>
        /// <param name="zipFilename"></param>
        /// <param name="outputFolder"></param>
        /// <returns></returns>
        public String UnzipFile(String outputFolder)
        {
            DirectoryInfo[] startDirectories = (new DirectoryInfo(outputFolder)).GetDirectories();
            String t = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + SevenZipFolderLocation;
            SevenZip.SevenZipBase.SetLibraryPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                + SevenZipFolderLocation);
            SevenZip.SevenZipExtractor ex = new SevenZip.SevenZipExtractor(zipFileLocation.FullName);
            ex.ExtractionFinished += (s, e) =>
            {
                logger.Info("Extracting finished");
            };

            ex.ExtractArchive(outputFolder);
            String parentFile = null;
            foreach (SevenZip.ArchiveFileInfo zipFile in ex.ArchiveFileData)
            {
                if (parentFile == null || parentFile.Length > zipFile.FileName.Length)
                {
                    parentFile = zipFile.FileName;
                }
            }

            if (parentFile.Contains("\\"))
            {
                parentFile = parentFile.Substring(0, parentFile.IndexOf("\\"));
            }

            return Path.Combine(outputFolder, parentFile);
        }
    }
}
