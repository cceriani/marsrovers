
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

The solution is formed by 4 projects

**Domain**

- This is the Domain project.

- It is a class library project.

- Contains all the models used in the application.

- Referenced by the Application project.

- Does not use any NuGet package.

**Application**

- This is the Application project.

- It is a class library project.

- Contains all the business logic.

- Referenced by the Application.UnitTest and the WebApi projects.

- Uses MediatR and FluentValidation NuGet packages.

------------- TODO: Explain about request, response, validator (with child) and handler

**Application.UnitTest**

- This is the Application unit tests project.

- It is a xUnit project.

- Contains all the unit tests for the application project.

- Uses xUnit and FluentAssertions NuGet packages.

**WebApi**

- This is the web API project.

- It is an API project.

- Contains all the controllers and a simple HTML page in wwwroot/index.html to test the functionality.

- Uses MediatR.Extensions.Microsoft.DependencyInjection, FluentValidation.AspNetCore, Swashbuckle.AspNetCore and Microsoft.AspNetCore.Mvc.NewtonsoftJson NuGet packages.


------------- TODO: Explain the MediatR, CQRS and FluentValidation patterns


### Assumptions

**Rover out of bounds**

- The author assumes that the Mars Rover can not go beyond the bounds of the grid.

- If this happens with any Mars Rover at any time, the application will return an error message.

**Initial list of rover instructions**

- The author decided to start the UI in index.html with a list of two rover instructions preset to make it easy the testing process.

- Anyway, the user can add new Rover instructions to the list or remove the existing ones



