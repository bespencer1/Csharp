using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using DataImport.Common;
using System.Data;
using System.Data.SqlClient;

namespace DataImport.DelimitedText
{
    public class Import : DataImport.Common.Import
    {
        private Config _config;
        private Encoding _defaultEncoding = Encoding.Default;

        //Default constructor
        public Import(string configFile)
        {
            _config = (Config)GetConfiguration(configFile, typeof(Config));
            ProcessImportFiles();
        }

        public override void ProcessImportFile(string importFile)
        {
            //Check row delimiter
            if (CheckRowDelimiter(importFile))
            {
                //Process the file
                using (StreamReader sr = new StreamReader(importFile, _defaultEncoding))
                {
                    using (DataTable data = new DataTable())
                    {
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_config.ConnectionString, SqlBulkCopyOptions.TableLock))
                        {
                            bulkCopy.DestinationTableName = _config.TableName;
                            bulkCopy.BulkCopyTimeout = 0;
                            bulkCopy.BatchSize = _config.BatchSize;

                            int rowCounter = 0;
                            int batchCounter = 0;
                            DataColumnCollection columns;

                            //Set the column position from the config file if no headers are in the data file
                            if (!_config.HasHeader)
                            {
                                columns = data.Columns;
                                foreach (Column fileColumn in _config.Columns)
                                {
                                    columns.Add(fileColumn.ColumnName, typeof(System.String));
                                }
                            }

                            while (sr.Peek() > 0)
                            {
                                try
                                {
                                    rowCounter++;
                                    string[] fileColumns = sr.ReadLine().Split(_config.ColumnDelimiter.ToCharArray());
                                    if (CheckColumnDelimiter(fileColumns))
                                    {
                                        //good format. process the data
                                        //had header row
                                        if (rowCounter == 1 && _config.HasHeader)
                                        {
                                            //TODO:  Confirm header exists in the file using the Display Name value

                                            columns = data.Columns;
                                            foreach (string fileColumn in fileColumns)
                                            {
                                                columns.Add(fileColumn, typeof(System.String));
                                            }
                                        }
                                        else
                                        {
                                            //Add the data rows
                                            data.Rows.Add(fileColumns);
                                            batchCounter++;

                                            //Once we reach the batch size, write the records to the table
                                            if (batchCounter == bulkCopy.BatchSize)
                                            {
                                                bulkCopy.WriteToServer(data);
                                                data.Rows.Clear();
                                                Console.WriteLine(string.Format("Writing {0} rows to the table", batchCounter.ToString()));
                                                batchCounter = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //incomplete row.  log and move on
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Error processing the record. log and move on
                                }
                            } //reader Loop

                            //Write any rows remaining
                            if (batchCounter > 0)
                            {                                
                                bulkCopy.WriteToServer(data);
                                data.Rows.Clear();
                            }

                        } //bulk copy
                    } // data table

                    //Close the import file
                    sr.Close();
                    sr.Dispose();

                } //stream reader
            }
            else
            {
                //cannot determine row delimiter.  log and move on
            }

        }

        private bool CheckRowDelimiter(string importFile)
        {

            bool retVal = true;

            //Open the file to read the contents
            string fileData = string.Empty;
            using (StreamReader sr = new StreamReader(importFile, _defaultEncoding))
            {
                fileData = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
            }

            
            //Check for we have records the meet one of the c# expected delimiters
            if (fileData.IndexOf("\r\n") <= 0 && fileData.IndexOf("\n") <= 0 && fileData.IndexOf("\n") <= 0)
            {
                //check to see if it has the delimiter in the settings file
                if (fileData.IndexOf(_config.RowDelimiter) > 0)
                {
                    //Replace the delimiter with one we want
                    fileData = fileData.Replace(_config.RowDelimiter, "\n");

                    //Move the original file to the archive
                    File.Move(importFile, Path.Combine(_config.ArchiveFolder, Path.GetFileName(importFile)) + ".orig");

                    //Write corrected file to working folder
                    using (StreamWriter sw = new StreamWriter(importFile, false, _defaultEncoding))
                    {
                        sw.Write(fileData);
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();
                    }
                }
                else
                {
                    //Cannot determine row delimiter, archive the file has bad
                    //Move the original file to the archive and mark as bad file
                    File.Move(importFile, Path.Combine(_config.ArchiveFolder, Path.GetFileName(importFile)) + ".bad");
                    retVal = false;
                }
            }

            return retVal;
        }

        private bool CheckColumnDelimiter(string[] columns)
        {
            bool retVal = true;

            //Check column count vs number of columns expected            
            if (columns.Length != _config.Columns.Count)
            {
                retVal = false;
            }

            return retVal;
        }

    }
}
