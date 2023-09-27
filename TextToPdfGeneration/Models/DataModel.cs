using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TextToPdfGeneration.Models
{
    public class DataModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string ConfirmationNumber { get; set; }
    }
}
