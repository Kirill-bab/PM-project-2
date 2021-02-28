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
                    return NoContent();
                case "alreadyOnPlatform":
                    return Conflict();
                case "wrongPassword" when tries >= 2:
                    {
                        await _gamingPlatform.BanUser(login);
                        return Unauthorized();
                    }
                case "wrongPassword":
                    return BadRequest();
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
            return NotFound();
        }

        [HttpGet]
        [Route("check/round")]
        public async Task<IActionResult> CheckRound([FromQuery] string login)
        {
            var isInGame = await _gamingPlatform.CheckIfInRound(login);
            if (isInGame) return Ok();
            return NotFound();
        }

        [HttpGet]
        [Route("round/result")]
        public async Task<IActionResult> CheckRoundResult([FromQuery] string login)
        {
            return await Task.Run<IActionResult>(async () =>
            {
                var result = await _gamingPlatform.GetLastRoundResult(login);
                switch (result)
                {
                    case "victory":
                        return Ok();
                    case "defeat":
                        return NotFound();
                    case "draw":
                        return NoContent();
                }
                return NotFound();
            });
        
        }

        [HttpGet]
        [Route("quit/game")]
        public async Task<IActionResult> QuitGame([FromQuery] string login)
        {
            await _gamingPlatform.QuitCurrentGame(login);
            return Ok();
        }

        [HttpGet]
        [Route("check/inqueue")]
        public async Task<IActionResult> CheckInQueue([FromQuery] string login)
        {
            var check = await _gamingPlatform.CheckIfInQueue(login);
            if (check) return Ok();
            return NotFound();
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
            if (await _gamingPlatform.ConfirmUserConnection(login))
                return Ok();
            else
                return NotFound();
        }

        [HttpGet]
        [Route("session/figure")]
        public Task<IActionResult> MakeTurn([FromQuery] string login,[FromQuery] string figure)
        {
            return Task.Run<IActionResult>(async () =>
            {
                _logger.LogWarning($"MakeTurn called by {login}");
                Figure fig = Figure.None;
                switch (figure)
                {
                    case "rock":
                        fig = Figure.Rock;
                        break;
                    case "paper":
                        fig = Figure.Paper;
                        break;
                    case "scissors":
                        fig = Figure.Scissors;
                        break;
                    default:
                        return BadRequest();
                }
                await _gamingPlatform.ChangeUserFigure(login, fig);
                return Ok();
            });
        }
    }
}
