using application.auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.src.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private ILogger<AuthController> _logger { get; }
        private IMediator _mediator { get; }

        public AuthController(ILogger<AuthController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Authentication([FromBody] AuthRequest auth)
        {
            _logger.LogInformation("Auth API is working!");
            var result = await _mediator.Send(auth);

            return await Task.FromResult(Ok(result));
        }
    }
}