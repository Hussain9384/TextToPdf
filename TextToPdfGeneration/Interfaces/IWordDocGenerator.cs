using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextToPdfGeneration.Models;

namespace TextToPdfGeneration.Interfaces
{
    internal interface IWordDocGenerator
    {
        void GenerateWordDocument(IEnumerable<(string fileName, DataModel dataModel)> fileNameAndContents);
    }
}
