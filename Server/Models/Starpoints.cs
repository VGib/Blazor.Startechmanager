using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Models
{
    public class Starpoints
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfPoints { get; set; }

        public string UrlJustification { get; set; }

        public string TextJustification { get; set; }
    }
}
