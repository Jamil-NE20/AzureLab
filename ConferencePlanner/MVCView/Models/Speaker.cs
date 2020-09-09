using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCView.Models
{
    public class Speaker
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Bio { get; set; }

        public virtual string WebSite { get; set; }
    }
}
