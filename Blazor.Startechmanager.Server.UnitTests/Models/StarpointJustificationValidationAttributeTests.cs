using Blazor.Startechmanager.Shared.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.Startechmanager.Server.UnitTests.Models
{
    public class StarpointJustificationValidationAttributeTests
    {
        [Test]
        public void A_starpoint_item_with_url_justification_should_be_validated()
        {
            var starpoint = new StarpointsItem
            {
                UrlJustification = "http://www.test.org"
            };
            ValidateSuccess(starpoint);
        }

        private static void ValidateSuccess(StarpointsItem starpoint)
        {
            var urlValidator = new StarpointItemJustficationValidationAttribute();
            urlValidator.Validate(starpoint.UrlJustification, new ValidationContext(starpoint));
            urlValidator.ErrorMessage.Should().BeNullOrEmpty();

            var textValidator = new StarpointItemJustficationValidationAttribute();
            textValidator.Validate(starpoint.TextJustification, new ValidationContext(starpoint));
            textValidator.ErrorMessage.Should().BeNullOrEmpty();
        }

        private static void ValidateFailure(StarpointsItem starpoint)
        {
            var urlValidator = new StarpointItemJustficationValidationAttribute();
            Action urlActionValidation = () => urlValidator.Validate(starpoint.UrlJustification, new ValidationContext(starpoint));
            urlActionValidation.Should().Throw<ValidationException>();

            var textValidator = new StarpointItemJustficationValidationAttribute();
            Action textActionValidation = () => textValidator.Validate(starpoint.TextJustification, new ValidationContext(starpoint));
            textActionValidation.Should().Throw<ValidationException>();
        }

        [Test]
        public void A_starpoint_item_with_text_justification_should_be_validated()
        {
            var starpoint = new StarpointsItem
            {
                TextJustification = "AAAAA"
            };
            ValidateSuccess(starpoint);
        }

        [Test]
        public void A_starpoint_item_no_url_justification__nor_text_justification_should_not_be_validated()
        {
            var starpoint = new StarpointsItem();
            ValidateFailure(starpoint);
        }
    }
}
