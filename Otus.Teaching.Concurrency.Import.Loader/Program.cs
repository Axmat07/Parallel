using System;
using System.Diagnostics;
using System.IO;
using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.DataGenerator.Generators;

namespace Otus.Teaching.Concurrency.Import.Loader
{
    class Program
    {
        private const string XmlFileName = "customers";
        private static string _dataFilePath = AppDomain.CurrentDomain.BaseDirectory!.Replace("Loader", "DataGenerator.App");
        private static bool _generateFromExe;
        private static int _dataCount = 1000;

        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("There are no args");
                return;
            }

            if (args.Length > 0)
            {
                if (args[0] != "exe" && args[0] != "proj")
                {
                    Console.WriteLine("First argument has to be \"exe\" or \"proj\"");
                    return;
                }
                
                if (args[0] == "exe")
                {
                    _generateFromExe = true;
                }
            }

            if (args.Length > 1)
            {
                if (!int.TryParse(args[1], out _dataCount))
                {
                    Console.WriteLine("Data must be integer");
                }
            }

            if (args.Length > 2)
            {
                _dataFilePath = args[2];
            }

            ProcessGeneration(_generateFromExe, _dataFilePath, XmlFileName, _dataCount);

            Console.WriteLine($"Loader started with process Id {Process.GetCurrentProcess().Id}...");

            var loader = new FakeDataLoader();

            loader.LoadData();
        }

        private static void ProcessGeneration(bool generateFromExe, string dataFilePath, string xmlFileName, int dataCount)
        {
            Console.WriteLine($"Started generating with {(generateFromExe ? "exe" : "internal project")}");

            if (generateFromExe)
            {
                GenerateCustomerDataFileFromExe(dataFilePath, xmlFileName, dataCount);
            }

            else
            {
                GenerateCustomersDataFile(dataFilePath, xmlFileName, dataCount);
            }
        }

        private static void GenerateCustomerDataFileFromExe(string dataFilePath, string xmlFileName, int dataCount)
        {
            var args = $"{xmlFileName} {dataCount}";
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(dataFilePath, "Otus.Teaching.Concurrency.Import.DataGenerator.App.exe"),
                Arguments = args
            };

            Process.Start(startInfo);
        }

        private static void GenerateCustomersDataFile(string dataFilePath, string xmlFileName, int dataCount)
        {
            var xmlGenerator = new XmlGenerator(Path.Combine(dataFilePath, $"{xmlFileName}.xml"), dataCount);
            xmlGenerator.Generate();
        }
    }
}