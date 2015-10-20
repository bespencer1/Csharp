using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SQL_Bulk_File_Import
{
    class Program
    {
        private static string _connectionString;
        private static string _tableName;
        private static string _importFile;
        private static string _rowDelimiter = "\r\n";
        private static string _columnDelimiter = ",";

        static void Main(string[] args)
        {
            //Get settings
            if (args.Length >= 2)
            {
                //Required settings
                _connectionString = args[0];
                _tableName = args[1];
                _importFile = args[2];

                //Optional settings
                if (args.Length >= 4)
                    _rowDelimiter = args[3];

                if (args.Length >= 5)
                    _columnDelimiter = args[4];

                //Verify the settings
                if (VerifySettings())
                {
                    //Process the data file
                    ProcessDataFile();
                }
            }
            else
            {
                //print usage information
                string usageInfo = "Usage information.\n\nExample: SQL_Bulk_File_Import.exe \"Server=<Server_Name>;Database=<Database_Name>;Trusted_Connection=yes;\" \"<Table_Name>\" \"<Data File>\"\n\n" +
                        "Parameters:\n" +
                        "1:  SQL Connection string\n"+
                        "2:  Import table name\n" +
                        "3:  Import file path.  The first column in file must contain column names\n"+
                        "4:  (optional) Row Delimiter.  Default is CR LF\n\n"+
                        "5:  (optional) Column Delimiter.  Deault is comma (,)";
                Console.WriteLine(usageInfo);

                Console.WriteLine("Press [Enter] key to continue....");
                Console.ReadLine();
            }
        }
        
        //Verify the values passed to the EXE have enough correct information to process the import file
        private static bool VerifySettings()
        {
            bool retVal = true;

            //Check connection string
            if(string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("SQL Connection String parameter is empty");
                retVal = false;
            }

            //Check import table name
            if (string.IsNullOrEmpty(_tableName))
            {
                Console.WriteLine("Import table name parameter is empty");
                retVal = false;
            }

            //Check import file
            if (string.IsNullOrEmpty(_importFile))
            {
                Console.WriteLine("Import file path parameter is empty");
                retVal = false;
            }
            else
            {
                if (!File.Exists(_importFile))
                {
                    Console.WriteLine("Import file does not exist at the pathe specified");
                    retVal = false;
                }
            }

            //Check row delimiter
            if (string.IsNullOrEmpty(_rowDelimiter))
            {
                Console.WriteLine("Row Delimiter parameter is empty");
                retVal = false;
            }

            //Check column delimiter
            if (string.IsNullOrEmpty(_columnDelimiter))
            {
                Console.WriteLine("Column Delimiter parameter is empty");
                retVal = false;
            }

            return retVal;
        }

        //Process the data file and import it into the SQL table.
        private static void ProcessDataFile()
        {
            try
            {
                //Record the time for diagnostic purposes
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();

                using (StreamReader reader = new StreamReader(_importFile))
                {
                    using (DataTable data = new DataTable())
                    {
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.TableLock))
                        {
                            bulkCopy.DestinationTableName = _tableName;
                            bulkCopy.BulkCopyTimeout = 0;
                            bulkCopy.BatchSize = 100000;

                            int rowCounter = 0;
                            int batchCounter = 0;
                            DataColumnCollection columns;

                            //Continue to read until end of file
                            while (!reader.EndOfStream)
                            {
                                try
                                {
                                    //Split the columns into a string array
                                    string[] fileColumns = reader.ReadLine().Split(_columnDelimiter.ToCharArray());
                                    rowCounter++;

                                    //If this is the first row, use it to determine the table columns
                                    if (rowCounter == 1)
                                    {
                                        columns = data.Columns;
                                        foreach (string fileColumn in fileColumns)
                                        {
                                            columns.Add(fileColumn, typeof(System.String));
                                        }
                                    }
                                    else
                                    {
                                        //Add all rows except for the first 1
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
                                catch (Exception readerEx)
                                {
                                    //Catch any errors with current read and move on to the next
                                    Console.WriteLine(readerEx.Message);
                                }
                            } //reader loop

                            //Write any rows remaining
                            bulkCopy.WriteToServer(data);
                            data.Rows.Clear();
                            Console.WriteLine(string.Format("Writing last {0} rows to the table", batchCounter.ToString()));

                            //Stop the timer
                            timer.Stop();
                            Console.WriteLine(string.Format("{0} rows imported in {1}",rowCounter.ToString(),timer.Elapsed.TotalSeconds.ToString()));


                        } //Bulk Copy
                    } //Data Table
                } //Stream Reader
            }
            catch (IOException ioEx)
            {
                Console.WriteLine(ioEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
