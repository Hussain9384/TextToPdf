﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextToPdfGeneration.Models;

namespace TextToPdfGeneration.Interfaces
{
    internal interface IFileOperations
    {
        public List<DataModel> ConvertLinesToDataModels(string filePath);
    }
}
