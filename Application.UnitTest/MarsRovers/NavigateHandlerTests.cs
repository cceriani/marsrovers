using System;
using System.Threading;
using System.Threading.Tasks;
using Application.MarsRovers;
using Domain.Enums;
using Domain.Models;
using FluentAssertions;
using Xunit;

namespace Application.UnitTest.MarsRovers
{
    public class NavigateHandlerTests
    {
        private NavigateRequest request;
        private readonly CancellationTokenSource CancellationToken;

        public NavigateHandlerTests()
        {
            request = new NavigateRequest
            {
                eastBound = 5,
                northBound = 5,

                RoverInstructions = new System.Collections.Generic.List<RoverInstruction>()
                {
                    new RoverInstruction
                    {
                        Rover = new Rover { x = 1, y = 2, heading = Domain.Enums.HeadingEnum.N },
                        instructions = "LMLMRMLMM"
                    },
                    new RoverInstruction
                    {
                        Rover = new Rover { x = 3, y = 3, heading = Domain.Enums.HeadingEnum.E },
                        instructions = "MMRMMRMRRM"
                    }
                }
            };

            CancellationToken = new CancellationTokenSource(1000);
        }

        #region Helpers
        public class NavigateHandlerAdapter : NavigateHandler
        {
            public Rover NavigateAdapter(RoverInstruction roverInstruction, int eastBound, int northBound)
                => Navigate(roverInstruction, eastBound, northBound);

            public Rover FollowInstructionAdapter(Rover current, char instruction) => FollowInstruction(current, instruction);

            public HeadingEnum RotateAdapter(HeadingEnum currentHeading, char instruction)
                => Rotate(currentHeading, instruction);

            public Rover AdvanceAdapter(Rover rover) => Advance(rover);

            public bool OutOfBoundsAdapter(int x, int y, int eastBound, int northBound)
                => OutOfBounds(x, y, eastBound, northBound);
        }
        #endregion


        #region Navigate handler tests
        [Theory]
        [InlineData(1, 2, HeadingEnum.N, "LMLMLMLMM", "13 N")]
        [InlineData(1, 2, HeadingEnum.N, "MMRMRMLLMMRMRM", "34 S")]
        [InlineData(1, 2, HeadingEnum.N, "LMRMMMRMMMMMRMMMMMRMMMMMRMMRML", "12 N")]
        [InlineData(3, 3, HeadingEnum.E, "MMRMMRMRRM", "51 E")]
        public async Task NavigateHandler_ShouldReturnCorrectResponse_WhenRequestIsOk(int x, int y,
            HeadingEnum heading, string instructions, string expectedResult)
        {
            //Arrange
            request.RoverInstructions[0].Rover.x = x;
            request.RoverInstructions[0].Rover.y = y;
            request.RoverInstructions[0].Rover.heading = heading;
            request.RoverInstructions[0].instructions = instructions;
            var _sut = new NavigateHandler();

            //Act
            var result = await _sut.Handle(request, CancellationToken.Token);

            //Assert
            result.Should().NotBeNull();
            result.error.Should().BeNull();
            result.Rovers.Should().NotBeNull();
            result.Rovers.Count.Should().Be(2);
            result.Rovers[0].positionAndHeading.Should().Be(expectedResult);
            result.Rovers[1].positionAndHeading.Should().Be("51 E");
        }

        [Fact]
        public async Task NavigateHandler_ShouldThrowException_WhenRequestIsNull()
        {
            //Arrange
            request = null;
            var _sut = new NavigateHandler();
            string error = null;
            NavigateResponse result;

            //Act

            try
            {
                result = await _sut.Handle(request, CancellationToken.Token);
            }
            catch (Exception e)
            {
                result = null;
                error = e.Message;
            }

            //Assert
            result.Should().BeNull();
            error.Should().NotBeNull();
            error.Should().Be("Wrong request");
        }

        [Fact]
        public async Task NavigateHandler_ShouldThrowException_WhenRequestValidationFails()
        {
            //Arrange

            //force validation errors
            request.RoverInstructions[0].instructions = "X";
            request.northBound = -1;
            var _sut = new NavigateHandler();
            string error = null;
            NavigateResponse result;

            //Act

            try
            {
                result = await _sut.Handle(request, CancellationToken.Token);
            }
            catch (Exception e)
            {
                result = null;
                error = e.Message;
            }

            //Assert
            result.Should().BeNull();
            error.Should().NotBeNull();
            error.Should().Contain("North bound must be greater than 0");
            error.Should().Contain("Invalid instructions found");
        }

        [Theory]
        [InlineData("MMMM")]
        [InlineData("LMMRRMMMM")]
        [InlineData("RMMMMM")]
        [InlineData("RRMMM")]
        [InlineData("RMMLMMLMRMM")]
        public async Task NavigateHandler_ShouldThrowException_WhenRoverGoesOutOfBounds(string movements)
        {
            //Arrange

            //force validation errors
            request.RoverInstructions[0].instructions = movements;
            var _sut = new NavigateHandler();
            string error = null;
            NavigateResponse result;

            //Act

            try
            {
                result = await _sut.Handle(request, CancellationToken.Token);
            }
            catch (Exception e)
            {
                result = null;
                error = e.Message;
            }

            //Assert
            result.Should().BeNull();
            error.Should().NotBeNull();
            error.Should().Be("Mars Rover is out of bounds");
        }
        #endregion


        #region Navigate tests
        [Fact]
        public void Navigate_ShouldReturnNull_WhenRoverIsNull()
        {
            //Arrange
            var _sut = new NavigateHandlerAdapter();

            //Act
            var result = _sut.NavigateAdapter(null, request.eastBound, request.northBound);

            //Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("MMMM")]
        [InlineData("LMMRRMMMM")]
        [InlineData("RMMMMM")]
        [InlineData("RRMMM")]
        [InlineData("RMMLMMLMRMM")]
        public void Navigate_ShouldThrowException_WhenRoverGoesOutOfBounds(string instructions)
        {
            //Arrange
            request.RoverInstructions[0].instructions = instructions;
            var _sut = new NavigateHandlerAdapter();
            Rover result = null;
            string error = null;

            //Act
            try
            {
                result = _sut.NavigateAdapter(request.RoverInstructions[0], request.eastBound, request.northBound);
            }
            catch (Exception e)
            {
                result = null;
                error = e.Message;
            }

            //Assert
            result.Should().BeNull();
            error.Should().NotBeNull();
            error.Should().NotBeNull();
            error.Should().Be("Mars Rover is out of bounds");
        }

        [Theory]
        [InlineData(1, 2, HeadingEnum.N, "LMLMLMLMM", "13 N")]
        [InlineData(1, 2, HeadingEnum.N, "MMRMRMLLMMRMRM", "34 S")]
        [InlineData(1, 2, HeadingEnum.N, "LMRMMMRMMMMMRMMMMMRMMMMMRMMRML", "12 N")]
        [InlineData(3, 3, HeadingEnum.E, "MMRMMRMRRM", "51 E")]
        public void Navigate_ShouldReturnRover_WhenEverythingIsOk(int x, int y, HeadingEnum heading, string instructions, string expectedResult)
        {
            //Arrange
            request.RoverInstructions[0].Rover.x = x;
            request.RoverInstructions[0].Rover.y = y;
            request.RoverInstructions[0].Rover.heading = heading;
            request.RoverInstructions[0].instructions = instructions;
            var _sut = new NavigateHandlerAdapter();

            //Act
            var result = _sut.NavigateAdapter(request.RoverInstructions[0], request.eastBound, request.northBound);

            //Assert
            result.Should().NotBeNull();
            result.positionAndHeading.Should().Be(expectedResult);
        }
        #endregion


        #region Follow instruction tests
        [Fact]
        public void FollowInstruction_ShouldReturnNull_WhenRoverIsNull()
        {
            //Arrange
            var _sut = new NavigateHandlerAdapter();

            //Act
            var result = _sut.FollowInstructionAdapter(null, 'L');

            //Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(HeadingEnum.N, 'L', 3, 3, HeadingEnum.W)]
        [InlineData(HeadingEnum.N, 'R', 3, 3, HeadingEnum.E)]
        [InlineData(HeadingEnum.N, 'M', 3, 4, HeadingEnum.N)]
        [InlineData(HeadingEnum.E, 'L', 3, 3, HeadingEnum.N)]
        [InlineData(HeadingEnum.E, 'R', 3, 3, HeadingEnum.S)]
        [InlineData(HeadingEnum.E, 'M', 4, 3, HeadingEnum.E)]
        [InlineData(HeadingEnum.S, 'L', 3, 3, HeadingEnum.E)]
        [InlineData(HeadingEnum.S, 'R', 3, 3, HeadingEnum.W)]
        [InlineData(HeadingEnum.S, 'M', 3, 2, HeadingEnum.S)]
        [InlineData(HeadingEnum.W, 'L', 3, 3, HeadingEnum.S)]
        [InlineData(HeadingEnum.W, 'R', 3, 3, HeadingEnum.N)]
        [InlineData(HeadingEnum.W, 'M', 2, 3, HeadingEnum.W)]
        public void FollowInstruction_ShouldReturnNewRover(HeadingEnum currentHeading, char instruction, int newX, int newY, HeadingEnum newHeading)
        {
            //Arrange
            var current = new Rover
            {
                x = 3,
                y = 3,
                heading = currentHeading
            };
            var _sut = new NavigateHandlerAdapter();

            //Act
            var result = _sut.FollowInstructionAdapter(current, instruction);

            //Assert
            result.Should().NotBeNull();
            result.x.Should().Be(newX);
            result.y.Should().Be(newY);
            result.heading.Should().Be(newHeading);
        }
        #endregion


        #region Rotate tests
        [Theory]
        [InlineData(HeadingEnum.N, 'L', HeadingEnum.W)]
        [InlineData(HeadingEnum.N, 'R', HeadingEnum.E)]
        [InlineData(HeadingEnum.N, 'M', HeadingEnum.N)]
        [InlineData(HeadingEnum.E, 'L', HeadingEnum.N)]
        [InlineData(HeadingEnum.E, 'R', HeadingEnum.S)]
        [InlineData(HeadingEnum.E, 'M', HeadingEnum.E)]
        [InlineData(HeadingEnum.S, 'L', HeadingEnum.E)]
        [InlineData(HeadingEnum.S, 'R', HeadingEnum.W)]
        [InlineData(HeadingEnum.S, 'M', HeadingEnum.S)]
        [InlineData(HeadingEnum.W, 'L', HeadingEnum.S)]
        [InlineData(HeadingEnum.W, 'R', HeadingEnum.N)]
        [InlineData(HeadingEnum.W, 'M', HeadingEnum.W)]
        public void Rotate_ShouldReturnNewHeading(HeadingEnum heading, char instruction, HeadingEnum expectedNewHeading)
        {
            //Arrange
            var _sut = new NavigateHandlerAdapter();

            //Act
            var result = _sut.RotateAdapter(heading, instruction);

            //Assert
            result.Should().Be(expectedNewHeading);
        }
        #endregion


        #region Advance tests
        [Fact]
        public void Advance_ShouldReturnNull_WhenRoverIsNull()
        {
            //Arrange
            var _sut = new NavigateHandlerAdapter();

            //Act
            var result = _sut.AdvanceAdapter(null);

            //Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(HeadingEnum.N, 3, 4)]
        [InlineData(HeadingEnum.E, 4, 3)]
        [InlineData(HeadingEnum.S, 3, 2)]
        [InlineData(HeadingEnum.W, 2, 3)]
        public void Advance_ShouldReturnNewRover(HeadingEnum heading, int expectedX, int expectedY)
        {
            //Arrange
            var rover = new Rover
            {
                x = 3,
                y = 3,
                heading = heading
            };
            var _sut = new NavigateHandlerAdapter();

            //Act
            var result = _sut.AdvanceAdapter(rover);

            //Assert
            result.Should().NotBeNull();
            result.x.Should().Be(expectedX);
            result.y.Should().Be(expectedY);
            result.heading.Should().Be(heading);
        }
        #endregion


        #region Out of bounds tests
        [Theory]
        [InlineData(-1, 0, 5, 5, true)]
        [InlineData(0, -1, 5, 5, true)]
        [InlineData(6, 0, 5, 5, true)]
        [InlineData(0, 6, 5, 5, true)]
        [InlineData(4, 4, 5, 5, false)]
        public void OutOfBound_ShouldReturnCorrectValue(int x, int y, int eastBound, int northBound, bool expectedResult)
        {
            //Arrange
            var _sut = new NavigateHandlerAdapter();

            //Act
            var result = _sut.OutOfBoundsAdapter(x, y, eastBound, northBound);

            //Assert
            result.Should().Be(expectedResult);
        }
        #endregion

    }
}
