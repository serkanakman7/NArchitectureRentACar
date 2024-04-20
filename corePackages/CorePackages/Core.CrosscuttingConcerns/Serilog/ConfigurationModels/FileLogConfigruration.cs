using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrosscuttingConcerns.Serilog.ConfigurationModels
{
    public class FileLogConfigruration
    {
        public string FolderPath { get; set; }

        public FileLogConfigruration()
        {
            FolderPath = string.Empty;
        }

        public FileLogConfigruration(string folderPath)
        {
            FolderPath = folderPath;
        }
    }
}
