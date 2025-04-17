using FluentValidation;
using AnastasiiaPortfolio.Models;

namespace AnastasiiaPortfolio.Validation
{
    public class PlayerScoreValidator : AbstractValidator<PlayerScore>
    {
        public PlayerScoreValidator()
        {
            RuleFor(x => x.PlayerName)
                .NotEmpty().WithMessage("Player name is required.")
                .MaximumLength(50).WithMessage("Player name cannot exceed 50 characters.");

            // Score is usually assigned by game logic, but you could add range validation if needed
            // RuleFor(x => x.Score).GreaterThanOrEqualTo(0);
        }
    }
} 