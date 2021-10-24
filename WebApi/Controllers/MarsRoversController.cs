using System;
using System.Threading.Tasks;
using Application.MarsRovers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarsRoversController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MarsRoversController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<NavigateResponse>> Post([FromBody] NavigateRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

    }
}
