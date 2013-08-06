using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace UnzipArtifact
{
    public class Unzipper
    {
        private const String SevenZipFolderLocation = @"\7-zip\7z.dll";
        private static ILog logger = LogManager.GetLogger(typeof(Unzipper));
        private FileInfo zipFileLocation;

        public Unzipper(FileInfo zipFileLocation)
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
