﻿using Blazor.Startechmanager.Shared.Models;

namespace Blazor.Startechmanager.Server.Models
{
    public class MappingStartechUser
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public Startechs Startech { get; set; }
        public bool IsLeader { get; set; }
    }
}
