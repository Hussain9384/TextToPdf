using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToWordGeneration.Models
{
    public class Config
    {
        public string[] FieldList { get; set; }
        public string OutputFileNameLookup { get; set; }
        public string Seperator { get; set; }
        public string MappingFileName { get; set; }

        public string[] MappingFieldList { get; set; }
    }
}