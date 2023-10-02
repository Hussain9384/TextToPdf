using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextToWordGeneration.Models;

namespace TextToWordGeneration.Interfaces
{
    internal interface IFileOperations
    {
        Task<List<DataModel>> ConvertLinesToDataModels(string filePath);

        void MoveAllFilesToZip(string sourceFolderPath, string zipFilePath);

        Task WriteDataModelsAsCsv(string outputFilePath, IEnumerable<(string outputFilePath, DataModel model, bool status)> successfullRecords);
    }
}
