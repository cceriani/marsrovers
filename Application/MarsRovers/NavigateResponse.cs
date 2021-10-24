using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Models;

namespace Application.MarsRovers
{
    public class NavigateResponse
    {
        public List<Rover> Rovers { get; set; }

        public string error { get; set; }
    }
}
