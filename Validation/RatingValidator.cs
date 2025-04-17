using FluentValidation;
using AnastasiiaPortfolio.Models;

namespace AnastasiiaPortfolio.Validation
{
    public class RatingValidator : AbstractValidator<Rating>
    {
        public RatingValidator()
        {
            RuleFor(x => x.RatingValue)
                .NotEmpty().WithMessage("Rating value is required.")
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
        }
    }
} 