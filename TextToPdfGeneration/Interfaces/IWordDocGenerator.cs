using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextToWordGeneration.Models;

namespace TextToWordGeneration.Interfaces
{
    internal interface IWordDocGenerator
    {
        Task<List<(string, DataModel, bool)>> GenerateWordDocument(IEnumerable<(string fileName, DataModel dataModel)> fileNameAndContents);
    }
}
