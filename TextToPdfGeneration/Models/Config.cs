using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToPdfGeneration.Models
{
    internal class Config
    {
        public string[] FieldList { get; set; }
        public string PdfFileContentFormat { get; set; }
        public string OutputFileNameLookup { get; set; }
        public string Seperator { get; set; }
    }
}