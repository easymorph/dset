using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyMorph.Drivers;

namespace DsetToCsv
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(
                    "Incorrect number of arguments. Expected two arguments: source file path and destination file path");
                return;
            }

            var sourceFile = args[0];
            var destinationFile = args[1];
            if (!File.Exists(sourceFile))
            {
                Console.WriteLine($"File does not exist: {sourceFile}");
                return;
            }

            if (File.Exists(destinationFile))
            {
                Console.WriteLine($"Destination file will be overwritten: {destinationFile}");
            }

            await ImportDsetExportCsv(new ImportConfig(sourceFile), destinationFile);
        }

        static async Task ImportDsetExportCsv(ImportConfig config, string destinationFile)
        {
            Console.WriteLine($"Started {config.FileName}");
            var importDriver = DriverFactory.GetImportDriver();
            var datasets = await importDriver.ImportAsync(config);
            if (datasets.Length == 0)
            {
                Console.WriteLine($"Source file is empty, nothing to export: {config.FileName}");
                return;
            }

            var dataset = datasets.First();
            CsvExportDriver.WriteData(dataset, destinationFile, ",", config.Token);

            Console.WriteLine($"Finished {config.FileName}");
        }
    }
}