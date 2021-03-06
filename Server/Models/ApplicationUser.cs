﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public int NumberOfPoints { get; set; }

        public List<MappingStartechUser> Startechs { get; set; } = new List<MappingStartechUser>();
    }

    public class ApplicationRole : IdentityRole<int>
    {
    }
}
