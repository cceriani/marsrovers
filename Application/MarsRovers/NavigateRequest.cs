using System.Collections.Generic;
using Domain.Models;
using MediatR;

namespace Application.MarsRovers
{
    public class NavigateRequest : IRequest<NavigateResponse>
    {
        public int northBound { get; set; }
        public int eastBound { get; set; }

        public List<RoverInstruction> RoverInstructions { get; set; }

        //public RoverPositionAndHeading positionAndHeading { get; set; }

        //public string movements { get; set; }
    }
}
