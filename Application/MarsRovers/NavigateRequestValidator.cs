using System;
using System.Linq;
using Application.ModelValidation;
using FluentValidation;

namespace Application.MarsRovers
{
    public class NavigateRequestValidator : AbstractValidator<NavigateRequest>
    {
        public NavigateRequestValidator()
        {
            RuleFor(x => x.eastBound)
                .GreaterThan(0).WithMessage("East bound must be greater than 0");

            RuleFor(x => x.northBound)
                .GreaterThan(0).WithMessage("North bound must be greater than 0");

            RuleFor(x => x.RoverInstructions)
                .NotEmpty().WithMessage("List of rover instructions can not be empty");

            RuleForEach(x => x.RoverInstructions)
                .NotNull().WithMessage("Rover instructions can not be null");

            RuleForEach(x => x.RoverInstructions)
                .SetValidator(x => new RoverInstructionValidator(x.eastBound, x.northBound));

        }
    }
}
