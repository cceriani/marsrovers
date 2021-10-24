
## Mars Rovers

### Problem

NASA intends to land robotic rovers on Mars to explore a particularly curious-looking plateau. The rovers must navigate this rectangular plateau in a way so that their on board cameras can get a complete image of the surrounding terrain to send back to Earth.

A simple two-dimensional coordinate grid is mapped to the plateau to aid in rover navigation. Each point on the grid is represented by a pair of numbers X Y which correspond to the number of points East or North, respectively, from the origin. The origin of the grid is represented by 0 0 which corresponds to the southwest corner of the plateau. 0 1 is the point directly north of 0 0, 1 1 is the point immediately east of 0 1, etc. A rover’s current position and heading are represented by a triple X Y Z consisting of its current grid position X Y plus a letter Z corresponding to one of the four cardinal compass points, N E S W. For example, 0 0 N indicates that the rover is in the very southwest corner of the plateau, facing north.

NASA remotely controls rovers via instructions consisting of strings of letters. Possible instruction letters are L, R, and M. L and R instruct the rover to turn 90 degrees left or right, respectively (without moving from its current spot), while M instructs the rover to move forward one grid point along its current heading.

Your task is write an application that takes the test input (instructions from NASA) and provides the expected output (the feedback from the rovers to NASA). Each rover will move in series, i.e. the next rover will not start moving until the one preceding it finishes.


#### INPUT


Assume the southwest corner of the grid is 0,0 (the origin). The first line of input establishes the exploration grid bounds by indicating the coordinates corresponding to the northeast corner of the plateau.
Next, each rover is given its instructions in turn. Each rover’s instructions consists of two lines of strings. The first string confirms the rover’s current position and heading. The second string consists of turn / move instructions.

**Test input**

- 55
- 12 N LMLMLMLMM
- 33 E MMRMMRMRRM


#### OUTPUT

Once each rover has received and completely executed its given instructions, it transmits its updated position and heading to NASA.

**Test output**

- 13 N
- 51 E


### DESIGN

The solution is formed by 4 projects and uses Mediator pattern and Fluent validations to reduce complexity

**Domain project**

- It is a class library project.

- Contains all the models used in the application.

- Referenced by the Application project.

- Doesn't use any NuGet package.

- There are only two models in this project. The model RoverInstruction that contains a Rover instance and a string with the navigation instructions, and the model Rover that contains the position and heading of a specific Mars Rover. Each model has its own Validators in the ModelNavigation folder of the Application project.

**Application project**

- It is a class library project.

- Contains all the business logic.

- Referenced by the Application.UnitTest and the WebApi projects.

- Uses MediatR and FluentValidation NuGet packages.

- This project has a MarsRovers folder that contains the request, response, validator and handler of the Navigate Rover command. When the MarsRovers controller from the WebApi sends a Mediator message with a NavigateRequest, the handler is called to execute all the logic involved in the operation and returns the NavigateResponse.

- The NavigateRequest contains the East and North bounds of the navigation grid, along with a list of Rover instructions.

- The NavigateResponse contains a list of Rovers that represents the new position and heading of each Rover after the Navigation, and an error string. When all the navigation instructions are executed successfully, error is null. Otherwise, error will contain the error message/s.

- The NavigateRequestValidator uses FluentValidation combined with their child validators from ModelValidation folder. Those are the RoverInstructionValidator and the RoverValidator.

**Application.UnitTest project**

- It is a xUnit project.

- Contains all the unit tests for the application project.

- Uses xUnit and FluentAssertions NuGet packages.

**WebApi project**

- It is an API project.

- Contains all the controllers and a simple HTML page in wwwroot/index.html to test the functionality.

- Uses MediatR.Extensions.Microsoft.DependencyInjection, FluentValidation.AspNetCore, Swashbuckle.AspNetCore and Microsoft.AspNetCore.Mvc.NewtonsoftJson NuGet packages.

- There is only one controller in this project, the MarsRoversController, with only one endpoint. It is a POST method that receives a NavigateRequest, validates it through FluentValidation and sends a Mediator message to reach the NavigateHandler. Then this method returns the NavigateResponse to the caller.

- The Startup of this project configures the following things:
  - A CORS policy to allow any origin, any method and anyheader.
  - NewtonSoftJson to allow enum values as strings
  - Fluent validation to be executed when a request is received in the controller
  - Swagger
  - MediatR to configure the NavigateHandler
  - A Transient DI of IValidator<NavigateRequest> with NavigateRequestValidator so that FluentValidator knows that an instance of NavigateRequest must be validated with an instance of NavigateRequestValidator.

- Inside the folder wwwroot there is an index.html file, set as the default launch url in launchSettings.json. This is the only page in the project and contains all the code needed to make manual testing to this application. It contains a form with some inputs to define the grid east and north bounds, and a section to create a list of rover instructions. In this section you can add as many rover instructions as you wish. When you finish with the grid and rover list defined, you can press the Submit button to call the API. All the client logic is written in pure javascript in the header of this file.


### Assumptions

**Rover out of bounds**

- The author assumes that the Mars Rover can not go beyond the bounds of the grid.

- If this happens with any Mars Rover at any time, the application will return an error message.

**Initial rover instructions**

- The author decided to start the UI in index.html with a list of two rover instructions preset to make it easy the testing process.

- Anyway, the user can add new Rover instructions to the list or remove the existing ones.

Author: Carlos Ceriani



