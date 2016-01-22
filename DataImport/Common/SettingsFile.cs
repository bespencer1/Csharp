using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace DataImport.Common
{
    public class SettingsFile : Database
    {
        private string _filePattern;
        private string _sourceFolder;
        private string _workingFolder;
        private string _archiveFolder;
        private FileTypes _fileType;

        [XmlAttribute("Name")]
        public string Name;

        [XmlAttribute("File_Pattern")]
        public string FilePattern {
            get { return _filePattern; }
            set { _filePattern = value; } 
        }

        [XmlAttribute("Source_Folder")]
        public string SourceFolder {
            get { return _sourceFolder; }
            set { _sourceFolder = CheckFolder(value); }
        }

        [XmlAttribute("Working_Folder")]
        public string WorkingFolder
        {
            get { return _workingFolder; }
            set { _workingFolder = CheckFolder(value); }
        }

        [XmlAttribute("Archive_Folder")]
        public string ArchiveFolder
        {
            get { return _archiveFolder; }
            set { _archiveFolder = CheckFolder(value); }
        }

        [XmlAttribute("Has_Header")]
        public bool HasHeader;

        [XmlAttribute("File_Type")]
        public FileTypes FileType { 
            get{ return _fileType;}
            set{ _fileType = value;}
        }

        //Types of files that can be processed
        public enum FileTypes
        {
            DelimitedText
            , FixedText
            , Excel
            , XML
            , JSON
        }

        public string[] GetSourceFiles()
        {
            return GetFiles(_sourceFolder, _filePattern);
        }

        public string[] GetWorkingFiles()
        {
            return GetFiles(_workingFolder, _filePattern);
        }

        private string[] GetFiles(string folder, string pattern)
        {
            string[] files = Directory.GetFiles(folder, pattern);
            return files;
        }

        //Check to see if the folder exists
        private string CheckFolder(string folder)
        {
            string retVal = string.Empty;

            //Check to see if the folder exists
            if (!Directory.Exists(folder))
            {
                //Try to create the directory
                try
                {
                    Directory.CreateDirectory(folder);
                }
                catch
                {
                    throw new IOException(string.Format("Folder ({0}) does not exist can cannot be created", folder));
                }
            }
            else
                retVal = folder;

            return retVal;
        }

    }
}
