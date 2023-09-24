using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToPdfGeneration.Interfaces
{
    internal interface IFileOperations
    {
        List<IEnumerable<string>> ReadAllFileLines(string filePath);
    }
}
