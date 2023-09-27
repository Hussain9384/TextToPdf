using System.Reflection;
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

        public List<DataModel> ConvertLinesToDataModels(string filePath)
        {
            List<DataModel> dataModels = new List<DataModel>();

            if (string.IsNullOrWhiteSpace(filePath)) { Console.WriteLine($"Received empty filePath: {filePath}"); return null; }

           string[] Rows = File.ReadAllLines(filePath);

            if (Rows.Count() < 0) { Console.WriteLine($"Received empty file: {filePath}"); return null; }

            Parallel.ForEach(Rows, row => {

                var listOfValues = row.Split(_configuration.Seperator);
                dataModels.Add(MapToDataModel(listOfValues));
            });

            return dataModels;
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
