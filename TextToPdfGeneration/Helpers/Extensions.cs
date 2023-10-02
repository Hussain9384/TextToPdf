using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text;
using TextToWordGeneration.Models;

namespace TextToPdfGeneration.Helpers
{
    public static class Extensions
    {
        public static string ConvertDataModelToString(this DataModel dataModel, Config config,string outputFileName,string seperatorToUse)
        {
            var propList = dataModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var proValueMap = propList.Select(s => new { s.Name , Value = s.GetValue(dataModel)?.ToString() ?? string.Empty });

            List<string> list = new List<string>();

            foreach (var item in config.MappingFieldList)
            {
                var propValue = proValueMap.FirstOrDefault(s => s.Name == item);

                if (propValue != null)
                {
                    list.Add(propValue.Value);
                }
            }

            list.Add(outputFileName);

            return string.Join(seperatorToUse, list.ToArray());
        }

        public static string GetFileName(this DataModel dataModel, Config config)
        {
            var lookup = config.OutputFileNameLookup;
            var lookupPosition = config.FieldList.ToList().IndexOf(lookup);

            var propertyInfo = dataModel.GetType().GetProperties().FirstOrDefault(s => s.Name == lookup);

            return propertyInfo != null ? (propertyInfo.GetValue(dataModel)?.ToString() ?? Guid.NewGuid().ToString()) : Guid.NewGuid().ToString();
        }
    }
}
