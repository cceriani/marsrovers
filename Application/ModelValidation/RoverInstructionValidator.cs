using System;
using Domain.Models;
using FluentValidation;

namespace Application.ModelValidation
{
    public class RoverInstructionValidator : AbstractValidator<RoverInstruction>
    {
        public RoverInstructionValidator(int eastBound, int northBound)
        {
            RuleFor(x => x.Rover)
                .NotNull().WithMessage("Rover can not be null");

            RuleFor(x => x.instructions)
                .Matches("^([LRM])*$").WithMessage("Invalid instructions found");

            RuleFor(x => x.Rover)
                .SetValidator(new RoverValidator(eastBound, northBound));
        }
    }
}
