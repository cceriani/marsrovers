using System.Linq;
using Application.MarsRovers;
using Domain.Models;
using FluentAssertions;
using Xunit;

namespace Application.UnitTest.MarsRovers
{
    public class NavigateValidationTests
    {
        private NavigateRequest model;

        public NavigateValidationTests()
        {
            model = new NavigateRequest
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
                        instructions = "LMRMMLM"
                    }
                }
            };
        }

        [Fact]
        public void NavigateRequestValidator_ShouldPass_WhenEverythingIsOk()
        {
            //Arrange
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NavigateRequestValidator_ShouldFail_WhenEastBoundIsNegative()
        {
            //Arrange
            model.eastBound = -1;
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeFalse();
            var errors = result.Errors.Select(x => x.ErrorMessage);
            errors.Should().Contain("East bound must be greater than 0");
        }

        [Fact]
        public void NavigateRequestValidator_ShouldFail_WhenNorthBoundIsNegative()
        {
            //Arrange
            model.northBound = -1;
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeFalse();
            var errors = result.Errors.Select(x => x.ErrorMessage);
            errors.Should().Contain("North bound must be greater than 0");
        }

        [Fact]
        public void NavigateRequestValidator_ShouldFail_WhenRoverInstructionListIsNull()
        {
            //Arrange
            model.RoverInstructions = null;
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeFalse();
            var errors = result.Errors.Select(x => x.ErrorMessage);
            errors.Should().Contain("List of rover instructions can not be empty");
        }

        [Fact]
        public void NavigateRequestValidator_ShouldFail_WhenRoverInstructionListIsEmpty()
        {
            //Arrange
            model.RoverInstructions = new System.Collections.Generic.List<RoverInstruction>();
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeFalse();
            var errors = result.Errors.Select(x => x.ErrorMessage);
            errors.Should().Contain("List of rover instructions can not be empty");
        }

        [Fact]
        public void NavigateRequestValidator_ShouldFail_WhenSomeRoverInstructionIsNull()
        {
            //Arrange
            model.RoverInstructions.Add(null);
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeFalse();
            var errors = result.Errors.Select(x => x.ErrorMessage);
            errors.Should().Contain("Rover instructions can not be null");
        }

        [Fact]
        public void NavigateRequestValidator_ShouldFail_WhenSomeRoverIsNull()
        {
            //Arrange
            model.RoverInstructions.Add(new RoverInstruction { instructions = "LLLL" });
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeFalse();
            var errors = result.Errors.Select(x => x.ErrorMessage);
            errors.Should().Contain("Rover can not be null");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void NavigateRequestValidator_ShouldPass_WhenInstructionsIsEmptyOrNull(string instructions)
        {
            //Arrange
            model.RoverInstructions[0].instructions = instructions;
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(-1, "Position x must be greater or equal than 0")]
        [InlineData(6, "Position x can not be greater than 5")]
        public void NavigateRequestValidator_ShouldFail_WhenRoverXPositionIsWrong(int x, string expectedError)
        {
            //Arrange
            model.RoverInstructions[0].Rover.x = x;
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeFalse();
            var errors = result.Errors.Select(x => x.ErrorMessage);
            errors.Should().Contain(expectedError);
        }

        [Theory]
        [InlineData(-1, "Position y must be greater or equal than 0")]
        [InlineData(6, "Position y can not be greater than 5")]
        public void NavigateRequestValidator_ShouldFail_WhenRoverYPositionIsWrong(int y, string expectedError)
        {
            //Arrange
            model.RoverInstructions[0].Rover.y = y;
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeFalse();
            var errors = result.Errors.Select(x => x.ErrorMessage);
            errors.Should().Contain(expectedError);
        }

        [Theory]
        [InlineData("LRMN")]
        [InlineData("A")]
        [InlineData(" LM")]
        [InlineData("LM ")]
        public void NavigateRequestValidator_ShouldFail_WhenInstructionsAreWrong(string instructions)
        {
            //Arrange
            model.RoverInstructions[0].instructions = instructions;
            var _sut = new NavigateRequestValidator();

            //Act
            var result = _sut.Validate(model);

            //Assert
            result.IsValid.Should().BeFalse();
            var errors = result.Errors.Select(x => x.ErrorMessage);
            errors.Should().Contain("Invalid instructions found");
        }

    }
}
