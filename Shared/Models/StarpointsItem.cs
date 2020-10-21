using System;
using System.ComponentModel.DataAnnotations;

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

        [StarpointItemJustficationValidation]
        public string? UrlJustification { get; set; }

        [StarpointItemJustficationValidation]
        public string? TextJustification { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class StarpointItemJustficationValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is StarpointsItem starpointItem && string.IsNullOrWhiteSpace(starpointItem.TextJustification)
                && string.IsNullOrWhiteSpace(starpointItem.UrlJustification))
            {
                return new ValidationResult("A justification is need", new[] { nameof(StarpointsItem.TextJustification), nameof(StarpointsItem.UrlJustification) });
            }

            return ValidationResult.Success;
        }
    }
}
