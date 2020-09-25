using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JamilDropBox.Models
{
    public class DropFile
    {
        public int Id { get; set; }
        public string NameFile { get; set; }

        public string FileSize { get; set; }

        public string Description { get; set; }
    }
}
