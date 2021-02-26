using GameWebApplication.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IGamingPlatform _gamingPlatform;

        public UserController(ILoggerFactory loggerFactory, IGamingPlatform gamingPlatform)
        {
            _logger = loggerFactory.CreateLogger<UserController>();
            _gamingPlatform = gamingPlatform;
        }

        [HttpGet]
        [Route("register")] // $"user/register?login={playerLogin}&password={playerPassword}"
        public async Task<IActionResult> Register([FromQuery] string login,[FromQuery] string password)
        {
            if(await _gamingPlatform.RegisterUserAsync(login, password))
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        [Route("authorize")] // $"user/authorize?login={playerLogin}&password={playerPassword}"
        public async Task<IActionResult> Authorise([FromQuery] string login, [FromQuery] string password, [FromQuery] int tries)
        {
            if (await _gamingPlatform.ConnectUserAsync(login, password))
            {
                return Ok();
            }
            if(tries == 2)
            {
                await _gamingPlatform.BanUser(login);
                return NotFound();
            }
            return BadRequest();
        }
    }
}
