using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextToPdfGeneration.Interfaces;
using TextToPdfGeneration.Models;

namespace TextToPdfGeneration.Processors
{
    internal class FileOperations : IFileOperations
    {
        public FileOperations(Config configuration)
        {
            _configuration = configuration;
        }

        public Config _configuration { get; }

        public List<IEnumerable<string>> ReadAllFileLines(string filePath)
        {
            List<IEnumerable<string>> lines = new List<IEnumerable<string>>();

            if (string.IsNullOrWhiteSpace(filePath)) { Console.WriteLine($"Received empty filePath: {filePath}"); return null; }

           string[] Rows = File.ReadAllLines(filePath);

            if (Rows.Count() < 0) { Console.WriteLine($"Received empty file: {filePath}"); return null; }

            Parallel.ForEach(Rows, row => {

                var listOfValues = row.Split(" ");

                var formatedLines = _configuration.FieldList.Select((s, i) =>
                string.Format(_configuration.PdfFileContentFormat, s, i <= listOfValues.Length ? listOfValues[i] : string.Empty));

                lines.Add(formatedLines);
            });

            return lines;
        }
    }
}
