using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataImport.DelimitedText;
using System.IO;

namespace RunImport
{
    class Program
    {
        static void Main(string[] args)
        {

            string configFolder = @"C:\Projects\github\Csharp\DataImport\RunImport\Config";
            string[] configFiles = Directory.GetFiles(configFolder);
            foreach (string file in configFiles)
            {
                try
                {
                    DataImport.DelimitedText.Import dtImport = new Import(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("DONE");
            Console.ReadLine();

        }
    }
}
