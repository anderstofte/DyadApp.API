﻿using System.Threading.Tasks;
using DyadApp.API.Extensions;
using DyadApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DyadApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MatchController : Controller
    {

        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpPost("AwaitingMatch")]
        public async Task<IActionResult> AwaitingMatch()
        {
            var userId = User.GetUserId();
            var isAddedToQueueOfAwaitingMatches = await _matchService.AddToAwaitingMatch(userId);
            
            if (!isAddedToQueueOfAwaitingMatches)
            {
                return BadRequest("Der gik noget galt.");
            }

            return Ok("Der er ikke nogle tilgængelige matches til dig lige nu. Vi arbejder på højtryk for at finde et!");
        }

        [HttpPost]
        public async Task<IActionResult> Match()
        {
            var userId = User.GetUserId();
            var isMatchFound = await _matchService.SearchForMatch(userId);

            if(!isMatchFound)
            {
                return BadRequest();
            }
            return Ok("Vi fandt et nyt match!");
        }
    }
}