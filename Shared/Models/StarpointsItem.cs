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
        
        [Range(1, int.MaxValue)]
        public int ApplicationUserId { get; set; }
        public DateTime Date { get; set; }

        [IsNotZero]
        
        public int NumberOfPoints { get; set; }

        [RequiredEnum(typeof(Startechs))]
        public Startechs Startech { get; set; }
        public StarpointsType? Type { get; set; }
        public int? StarpointsTypeId { get; set; }

        [RequiredEnum(typeof(ValidationState))]
        public ValidationState ValidationState { get; set; }

        [Url]
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
                return new ValidationResult("A justification is need", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IsNotZeroAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is int intValue && intValue == 0)
            {
                return new ValidationResult("value should not be 0", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredEnumAttribute : ValidationAttribute
    {
        private readonly Type valueType;
        public RequiredEnumAttribute(Type valueType)
        {
            this.valueType = valueType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (!Enum.IsDefined(valueType, value))
                {
                    return new ValidationResult($"value type {value} is not a defined value for enum {valueType.Name}", new[] { validationContext.MemberName });
                }

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult($"unknown exception {ex.Message}");
            }
        }
    }
}
