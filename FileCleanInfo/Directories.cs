using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCleanInfo
{
    public static class Directories
    {
        static DirectoryInfo _fileDirectoryInput;
        static DirectoryInfo _fileDirectoryOutput;

        static Directories()
        {
            _fileDirectoryInput = new DirectoryInfo("S:/Exchange/-/1/");
            _fileDirectoryOutput = new DirectoryInfo("S:/Exchange/-/2/");
        }

        public static DirectoryInfo FileDirectoryInput
        {
            get { return _fileDirectoryInput; }
            set { _fileDirectoryInput = value; }
        }

        public static DirectoryInfo FileDirectoryOutput
        {
            get { return _fileDirectoryOutput; }
            set { _fileDirectoryOutput = value; }
        }

        public static void ChangeDirectoryInput(string fileDirectory, bool isDirectotyInput)
        {
            if (isDirectotyInput == true)
                _fileDirectoryInput = new DirectoryInfo(fileDirectory);
            else
                _fileDirectoryInput = new DirectoryInfo(fileDirectory);
        }
        public static List<FileInfo> GetFiles()
        {
            return _fileDirectoryInput.GetFiles().ToList();
        }
        public static string GetOutput()
        {
            return _fileDirectoryOutput.ToString();
        }
    }
}
