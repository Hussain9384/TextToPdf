using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reflection;
using TextToPdfGeneration.Helpers;
using TextToWordGeneration.Interfaces;
using TextToWordGeneration.Models;

namespace TextToWordGeneration.Processors
{
    internal class FileOperations : IFileOperations
    {
        public FileOperations(Config configuration)
        {
            _configuration = configuration;
        }

        public Config _configuration { get; }

        public async Task<List<DataModel>> ConvertLinesToDataModels(string filePath)
        {
            ConcurrentBag<DataModel> dataModels = new ConcurrentBag<DataModel>();

            if (string.IsNullOrWhiteSpace(filePath)) { Console.WriteLine($"Received empty filePath: {filePath}"); return null; }

           string[] Rows = await File.ReadAllLinesAsync(filePath);

            if (Rows.Count() < 0) { Console.WriteLine($"Received empty file: {filePath}"); return null; }

            Parallel.ForEach(Rows, row => {

                var listOfValues = row.Split(_configuration.Seperator);
                dataModels.Add(MapToDataModel(listOfValues));
            });

            return dataModels.ToList();
        }

        public async Task WriteDataModelsAsCsv(string outputFilePath, IEnumerable<(string outputFilePath, DataModel model, bool status)> successfullRecords)
        {
            List<string> lines = new List<string>
            {
                string.Join(",", _configuration.MappingFieldList.Append<string>("OutputFile"))
            };

            foreach (var item in successfullRecords)
            {
                lines.Add(item.model.ConvertDataModelToString(_configuration, Path.GetFileName(item.outputFilePath), ","));
            }

            await File.WriteAllLinesAsync(Path.Combine(outputFilePath, $"{_configuration.MappingFileName}.csv"), lines.ToArray());
        }

        public void MoveAllFilesToZip(string sourceFolderPath, string zipFilePath)
        {
            try
            {
                if (!Directory.Exists(sourceFolderPath))
                {
                    Console.WriteLine("Source folder does not exist.");
                }

                using (FileStream zipStream = new FileStream(zipFilePath, FileMode.OpenOrCreate))
                {
                    using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                    {
                        AddFolderToZip(sourceFolderPath, archive, "");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error zipping folder: {ex.Message}");
            }
        }

        private static void AddFolderToZip(string sourceFolderPath, ZipArchive archive, string folderPathInArchive)
        {
            foreach (string filePath in Directory.GetFiles(sourceFolderPath))
            {
                string entryName = Path.Combine(folderPathInArchive, Path.GetFileName(filePath));
                archive.CreateEntryFromFile(filePath, entryName);
            }

            foreach (string subfolderPath in Directory.GetDirectories(sourceFolderPath))
            {
                string folderName = Path.GetFileName(subfolderPath);
                string entryName = Path.Combine(folderPathInArchive, folderName);
                AddFolderToZip(subfolderPath, archive, entryName);
            }
        }

        private DataModel MapToDataModel(string[] listOfValues)
        {
            var dataModel = new DataModel();

            var propertyValues = _configuration.FieldList.Select((s, i) => new { field = s, value = i < listOfValues.Length ? listOfValues[i] : string.Empty })
                .ToDictionary(s => s.field, s => s.value);

            Type objectType = dataModel.GetType();
            PropertyInfo[] properties = objectType.GetProperties();

            foreach (var property in properties)
            {
                if (propertyValues.TryGetValue(property.Name, out string value))
                {
                    if (property.PropertyType == value.GetType() || property.PropertyType.IsAssignableFrom(value.GetType()))
                    {
                        property.SetValue(dataModel, value);
                    }
                    else
                    {
                        Console.WriteLine($"Unable to set the property value for {property.Name}");
                    }
                }
            }

            return dataModel;
        }
    }
}
