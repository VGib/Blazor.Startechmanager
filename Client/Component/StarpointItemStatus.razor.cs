using Blazor.Startechmanager.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Component
{
    public partial class StarpointItemStatus
    {
        [Parameter]
        public StarpointsItem   Item { get; set; }
    }
}
