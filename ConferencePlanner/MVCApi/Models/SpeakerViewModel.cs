using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApi.Models
{
    public class SpeakerViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Bio { get; set; }

        public virtual string WebSite { get; set; }
    }
}