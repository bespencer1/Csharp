using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataImport.Common
{
    public class Import
    {
        private SettingsFile _file;

        //Default constructor
        public Import()
        {
            //TODO:  Confirm processing stored proc exists. if not, stop processing any files until install is complete
            //TODO:  Confirm batch table exists, if not create it
            //TODO:  Confirm validation errors table exists, if not create it
            //TODO:  Confirm stage table exists, if not create it
            
        }

        //Get the configuration information from the XML file
        public object GetConfiguration(string configFile, Type type)
        {
            //Deserizlize the XML settings file into an object that implements the File class
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
            FileStream fs = new FileStream(configFile, FileMode.Open, FileAccess.Read);
            object retVal = serializer.Deserialize(fs);

            //Store the File class values
            _file = (SettingsFile)retVal;

            return retVal;
        }

        private void MoveSourceToWorking()
        {
            string[] sourceFiles = _file.GetSourceFiles();
            foreach (string sourceFile in sourceFiles)
            {
                string fileName = Path.GetFileName(sourceFile);
                File.Move(sourceFile, Path.Combine(_file.WorkingFolder, fileName));
            }
        }

        public void ProcessImportFiles()
        {
            //Move files from source to working directory
            MoveSourceToWorking();

            //Process working files
            string[] workingFiles = _file.GetWorkingFiles();
            foreach (string workingFile in workingFiles)
            {
                //Record the time for diagnostic purposes
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();

                //Process the file
                ProcessImportFile(workingFile);

                //Archive the file
                File.Move(workingFile, Path.Combine(_file.WorkingFolder, Path.GetFileName(workingFile)));

                //Stop timer
                timer.Stop();
            }
        }

        public virtual void ProcessImportFile(string importFile)
        {
            throw new Exception("Must override");
        }
    }
}
