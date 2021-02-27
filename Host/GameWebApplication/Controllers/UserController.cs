using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            switch (await _gamingPlatform.ConnectUserAsync(login, password))
            {
                case "ok":
                    return Ok();
                case "banned":
                    return BadRequest();
                case "alreadyOnPlatform":
                    return Conflict();
                case "notFound" when tries >= 2:
                    {
                        await _gamingPlatform.BanUser(login);
                        return Unauthorized();
                    }            
                default: return NotFound();
            }
        }
        [HttpGet]
        [Route("disconnect")]
        public async Task<IActionResult> Disconnect([FromQuery] string login)
        {
            await _gamingPlatform.DisconnectUserAsync(login);
            return Ok();
        }
        [HttpGet]
        [Route("stats")]
        public async Task<IActionResult> GetStats([FromQuery] string login)
        {
            var stats = await _gamingPlatform.GetUserStatistics(login);
            return Ok(stats);
        }
        [HttpGet]
        [Route("session/stop/search")]
        public async Task<IActionResult> StopGame([FromQuery] string login)
        {
            await _gamingPlatform.StopSearch(login);
            return Ok();
        }
        [HttpGet]
        [Route("session/start/random")]
        public async Task<IActionResult> StartRandomGame([FromQuery] string login)
        {
            await _gamingPlatform.StartRandomSessionAsync(login);
            return Ok();
        }
        [HttpGet]
        [Route("check/game")]
        public async Task<IActionResult> CheckGame([FromQuery] string login)
        {
            var isInGame = await _gamingPlatform.IsInGame(login);
            if (isInGame) return Ok();
            return NoContent();

        }
        [HttpGet]
        [Route("start/private")]
        public async Task<IActionResult> StartPrivateGame([FromQuery] string login)
        {
           var gameKey = await _gamingPlatform.StartPrivateSessionAsync(login);
           return Ok(gameKey);
        }
        [HttpGet]
        [Route("start/ai")]
        public async Task<IActionResult> StartAIGame([FromQuery] string login)
        {
            await _gamingPlatform.StartAISessionAsync(login);
            return Ok();
        }
        [HttpGet]
        [Route("connect/game")]
        public async Task<IActionResult> ConnectToPrivateGame([FromQuery] string login, [FromQuery] string gameKey)
        {
            var isSuccessfull = await _gamingPlatform.ConnectToPrivateSessionAsync(login, gameKey);
            if (isSuccessfull) return Ok();
            return NotFound();
        }
        [HttpGet]
        [Route("stats/global")]
        public async Task<IActionResult> GetGlobalStats()
        {
            return Ok(await _gamingPlatform.GetGlobalStatistics()); 
        }
        [HttpGet]
        [Route("confirm/connection")]
        public async Task<IActionResult> ConfirmConnection([FromQuery] string login)
        {
            await _gamingPlatform.ConfirmUserConnection(login);
            return Ok();
        }
        [HttpGet]
        [Route("session/figure")]
        public Task<IActionResult> MakeTurn([FromQuery] string login,[FromQuery] string figure)
        {
            return Task.Run<IActionResult>(async () =>
            {
                Figure fig;
                switch (figure)
                {
                    case "rock":
                        fig = Figure.Rock;
                        break;
                    case "paper":
                        fig = Figure.Rock;
                        break;
                    case "scissors":
                        fig = Figure.Rock;
                        break;
                }
                //await _gamingPlatform.;
                return Ok();
            });
            
        }
    }
}
