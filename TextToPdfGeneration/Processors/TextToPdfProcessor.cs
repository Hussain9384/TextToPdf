using Microsoft.Extensions.Configuration;
using TextToPdfGeneration.Interfaces;
using TextToPdfGeneration.Models;

namespace TextToPdfGeneration.Processors
{
    internal class TextToPdfProcessor
    {
        private readonly Config _configuration;
        private readonly IFileOperations _fileOperations;
        private readonly IPdfGenerator _pdfGenerator;

        public TextToPdfProcessor(Config configuration,
                                  IFileOperations fileOperations,
                                  IPdfGenerator pdfGenerator )
        {
            _configuration = configuration;
            _fileOperations = fileOperations;
            _pdfGenerator = pdfGenerator;
        }
        public void Processor(string[] args) {

            if ( args.Length == 0 ) { Console.WriteLine("No input / outpur Arguments received to proceed!"); return; }

            var fileToProcess = args[0];
            var outputPath = args[1];

            Console.WriteLine($"Processing file : {fileToProcess}");
            Console.WriteLine($"outputPath : {outputPath}");

            var linesToProcess = _fileOperations.ReadAllFileLines(fileToProcess);

            IEnumerable<(string, IEnumerable<string>)> fileWithContents = linesToProcess.Select(s => (Path.Combine(outputPath,(getFileName(s))), s));

            _pdfGenerator.GeneratePdfFiles(fileWithContents);

        }

        private string getFileName(IEnumerable<string> content)
        {
            var lookup = _configuration.OutputFileNameLookup;
            var lookupPosition = _configuration.FieldList.ToList().IndexOf(lookup);

            return lookupPosition > 1 ? (GetTrimmedElement(content, lookupPosition)) : Guid.NewGuid().ToString();
        }

        private string GetTrimmedElement(IEnumerable<string> content, int lookupPosition)
        {
            var element = content.ElementAtOrDefault(lookupPosition);

            if (string.IsNullOrWhiteSpace(element)) {
                return Guid.NewGuid().ToString();
            }

            return element.Split(_configuration.Seperator).Last() + ".pdf";
        }
    }
}
