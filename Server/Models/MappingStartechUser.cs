using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Models
{
    public class MappingStartechUser
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public Startechs Startech { get; set; }
    }
}
