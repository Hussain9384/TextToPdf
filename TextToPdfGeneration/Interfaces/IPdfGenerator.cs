using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToPdfGeneration.Interfaces
{
    internal interface IPdfGenerator
    {
        void GeneratePdfFiles(IEnumerable<(string fileName, IEnumerable<string> fileContent)> fileWithContents);
        bool WriteFileStreamAsPdf(string outputPath, IEnumerable<string> fileRowContent);
    }
}
