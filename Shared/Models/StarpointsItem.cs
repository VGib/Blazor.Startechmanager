using System;

namespace Blazor.Startechmanager.Shared.Models
{
    public enum ValidationState
    {
        Validated = 1,
        Refused = 2,
        InStudy = 3
    }

    public class StarpointsItem
    {
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfPoints { get; set; }

        public Startechs Startech { get; set; }
        public StarpointsType? Type { get; set; }
        public int? StarpointsTypeId { get; set; }

        public ValidationState ValidationState { get; set; }

        public string? UrlJustification { get; set; }

        public string? TextJustification { get; set; }
    }
}
