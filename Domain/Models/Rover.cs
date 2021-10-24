using System;
using Domain.Enums;

namespace Domain.Models
{
    public class Rover
    {
        public int x { get; set; } //horizontal
        public int y { get; set; } //vertical

        public HeadingEnum heading { get; set; }

        public string positionAndHeading => $"{x}{y} {heading}";
    }
}
