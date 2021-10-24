using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using MediatR;

namespace Application.MarsRovers
{
    public class NavigateHandler : IRequestHandler<NavigateRequest, NavigateResponse>
    {
        public NavigateHandler()
        {
        }

        public Task<NavigateResponse> Handle(NavigateRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new Exception("Wrong request");
            }

            var validation = new NavigateRequestValidator().Validate(request);
            if (!validation.IsValid)
            {
                throw new Exception(string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)));
            }

            Rover rover;
            var response = new NavigateResponse { Rovers = new System.Collections.Generic.List<Rover>() };

            foreach (var roverInstruction in request.RoverInstructions)
            {
                rover = Navigate(roverInstruction, request.eastBound, request.northBound);
                if (rover == null)
                {
                    //this should never happen
                    throw new Exception("Navigation error: wrong request");
                }

                response.Rovers.Add(rover);
            }

            return Task.FromResult(response);
        }

        protected Rover Navigate(RoverInstruction roverInstruction, int eastBound, int northBound)
        {
            if (roverInstruction == null)
            {
                return null;
            }

            //request is valid, because it passed the NavigateRequestValidator

            var rover = roverInstruction.Rover;
            var instructions = roverInstruction.instructions.ToCharArray();

            foreach (var instruction in instructions)
            {
                rover = FollowInstruction(rover, instruction);
                if (OutOfBounds(rover.x, rover.y, eastBound, northBound))
                {
                    throw new Exception("Mars Rover is out of bounds");
                    //return new NavigateResponse { error = "Mars Rover is out of bounds" };
                }

            }

            return rover;
        }

        protected bool InstructionIsMovement(char instruction) => instruction.Equals('M');
        protected bool InstructionIsRotation(char instruction) => (instruction.Equals('L') || instruction.Equals('R'));

        protected Rover FollowInstruction(Rover current, char instruction)
        {
            if (current == null)
            {
                return null;
            }

            if (InstructionIsRotation(instruction))
            {
                current.heading = Rotate(current.heading, instruction);
            }

            if (InstructionIsMovement(instruction))
            {
                current = Advance(current);
            }

            return current;
        }

        protected HeadingEnum Rotate(HeadingEnum currentHeading, char instruction)
        {
            if (instruction.Equals('L'))
            {
                switch (currentHeading)
                {
                    case HeadingEnum.N: return HeadingEnum.W;
                    case HeadingEnum.E: return HeadingEnum.N;
                    case HeadingEnum.S: return HeadingEnum.E;
                    case HeadingEnum.W: return HeadingEnum.S;
                }
            }
            if (instruction.Equals('R'))
            {
                switch (currentHeading)
                {
                    case HeadingEnum.N: return HeadingEnum.E;
                    case HeadingEnum.E: return HeadingEnum.S;
                    case HeadingEnum.S: return HeadingEnum.W;
                    case HeadingEnum.W: return HeadingEnum.N;
                }
            }
            return currentHeading;
        }

        protected Rover Advance(Rover rover)
        {
            if (rover == null)
            {
                return null;
            }

            switch (rover.heading)
            {
                case HeadingEnum.N:
                    rover.y++;
                    break;
                case HeadingEnum.E:
                    rover.x++;
                    break;
                case HeadingEnum.S:
                    rover.y--;
                    break;
                case HeadingEnum.W:
                    rover.x--;
                    break;
            }

            return rover;
        }

        protected bool OutOfBounds(int x, int y, int eastBound, int northBound)
            => ((x < 0) || (x > eastBound) || (y < 0) || (y > northBound));
    }
}
