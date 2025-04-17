using FluentValidation;
using AnastasiiaPortfolio.Models;

namespace AnastasiiaPortfolio.Validation
{
    public class ReviewValidator : AbstractValidator<Review>
    {
        public ReviewValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Your name is required.");

            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage("Rating is required.")
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Pros)
                .MaximumLength(500).WithMessage("Pros cannot exceed 500 characters.");

            RuleFor(x => x.Cons)
                .MaximumLength(500).WithMessage("Cons cannot exceed 500 characters.");
        }
    }
} 