using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JamilDropBox.Models
{
    public class DropFile
    {
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string ModifiedOn { get; set; }
        public string Descriptions { get; set; }
    }
}
