using Microsoft.Extensions.Configuration;
using System.Reflection;
using TextToPdfGeneration.Interfaces;
using TextToPdfGeneration.Models;

namespace TextToPdfGeneration.Processors
{
    internal class TextToPdfProcessor
    {
        private readonly Config _configuration;
        private readonly IFileOperations _fileOperations;
        private readonly IWordDocGenerator _pdfGenerator;

        public TextToPdfProcessor(Config configuration,
                                  IFileOperations fileOperations,
                                  IWordDocGenerator pdfGenerator )
        {
            _configuration = configuration;
            _fileOperations = fileOperations;
            _pdfGenerator = pdfGenerator;
        }
        public void Processor(string[] args) {

           // if ( args.Length == 0 ) { Console.WriteLine("No input / outpur Arguments received to proceed!"); return; }

            var fileToProcess = "F:\\Temp\\Word\\text.txt"; //args[0]; 
            var outputPath = "F:\\Temp\\Word\\"; //args[1];

            Console.WriteLine($"Processing file : {fileToProcess}");
            Console.WriteLine($"outputPath : {outputPath}");

            var dataModels = _fileOperations.ConvertLinesToDataModels(fileToProcess);

            List<(string, DataModel)> fileNameAndContent = dataModels.Select(s => ( Path.Combine(outputPath,getFileName(s)), s)).ToList();

            _pdfGenerator.GenerateWordDocument(fileNameAndContent);

        }

        private string getFileName(DataModel dataModel)
        {
            var lookup = _configuration.OutputFileNameLookup;
            var lookupPosition = _configuration.FieldList.ToList().IndexOf(lookup);

            var propertyInfo = dataModel.GetType().GetProperties().FirstOrDefault(s => s.Name == lookup);

            return propertyInfo != null ? (propertyInfo.GetValue(dataModel)?.ToString() ?? Guid.NewGuid().ToString()) : Guid.NewGuid().ToString();
        }
    }
}
