using System;
using Domain.Models;
using FluentValidation;

namespace Application.ModelValidation
{
    public class RoverValidator : AbstractValidator<Rover>
    {
        public RoverValidator(int eastBound, int northBound)
        {
            RuleFor(x => x.x)
                .LessThanOrEqualTo(x => eastBound).WithMessage(x => $"Position x can not be greater than {eastBound}")
                .GreaterThanOrEqualTo(0).WithMessage(x => $"Position x must be greater or equal than 0");

            RuleFor(x => x.y)
                .LessThanOrEqualTo(x => northBound).WithMessage(x => $"Position y can not be greater than {northBound}")
                .GreaterThanOrEqualTo(0).WithMessage(x => $"Position y must be greater or equal than 0");
        }
    }
}
