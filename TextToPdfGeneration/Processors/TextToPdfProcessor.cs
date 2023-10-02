using TextToWordGeneration.Interfaces;
using TextToWordGeneration.Models;
using TextToPdfGeneration.Helpers;

namespace TextToWordGeneration.Processors
{
    internal class TextToWordProcessor
    {
        private readonly Config _configuration;
        private readonly IFileOperations _fileOperations;
        private readonly IWordDocGenerator _wordGenerator;

        public TextToWordProcessor(Config configuration,
                                  IFileOperations fileOperations,
                                  IWordDocGenerator wordGenerator )
        {
            _configuration = configuration;
            _fileOperations = fileOperations;
            _wordGenerator = wordGenerator;
        }
        public async Task Processor(string[] args) {

            //if ( args.Length == 0 ) { Console.WriteLine("No input / outpur Arguments received to proceed!"); return; }

            var fileToProcess = "F:\\Temp\\Word\\text.txt"; //args[0]; 
            var outputPath = "F:\\Temp\\Word\\"; //args[1];

            Console.WriteLine($"Processing file : {fileToProcess}");
            Console.WriteLine($"outputPath : {outputPath}");

            var uniquePath = Guid.NewGuid().ToString("N");
            var tempOutputPath = Path.Combine( outputPath, uniquePath );

            Directory.CreateDirectory(tempOutputPath);

            List<DataModel> dataModels = await _fileOperations.ConvertLinesToDataModels(fileToProcess);

            List<(string fileName, DataModel model)> fileNameAndContent = dataModels.Select(dataModel => ( Path.Combine(tempOutputPath, dataModel.GetFileName(_configuration)), dataModel)).ToList();

            List<(string outputFilePath, DataModel model, bool status)> result = await _wordGenerator.GenerateWordDocument(fileNameAndContent);

            var successfullRecords = result.Where(s => s.status);

            await _fileOperations.WriteDataModelsAsCsv(tempOutputPath, successfullRecords);

            _fileOperations.MoveAllFilesToZip(tempOutputPath, Path.Combine(outputPath, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip"));

            Directory.Delete(tempOutputPath, true );
        }
    }
}
