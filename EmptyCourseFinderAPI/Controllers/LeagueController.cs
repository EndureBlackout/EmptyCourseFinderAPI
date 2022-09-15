using EmptyCourseFinderAPI.Interfaces;
using EmptyCourseFinderAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmptyCourseFinderAPI.Concretes
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeagueController : Controller
    {
        private readonly ILeagueService _leagueService;

        public LeagueController(ILeagueService leagueService) {
            _leagueService = leagueService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IList<League>>> GetAllLeagues()
        {
            var leagues = await _leagueService.GetAllLeagues();

            return Ok(leagues);
        }

        [HttpGet]
        public async Task<ActionResult<League>> GetLeague(string leagueId)
        {
            var league = await _leagueService.GetLeagueDetails(leagueId);

            if(league == null)
            {
                return NotFound();
            }

            return Ok(league);
        }

        [HttpPost]
        public async Task<ActionResult<League>> CreateLeague(League league)
        {
            var creatorId = GetUserId();

            if(string.IsNullOrEmpty(creatorId))
            {
                return Unauthorized();
            }

            var newLeague = await _leagueService.CreateLeague(league.LeagueName, league.Description, league.LeagueTime, league.Location, league.Open, creatorId);

            if(newLeague == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(_leagueService.CreateLeague), newLeague);
        }

        [HttpPatch]
        public async Task<ActionResult<League>> UpdateLeague(LeagueUpdateRequest leagueUpdate)
        {
            var league = await _leagueService.GetLeagueDetails(leagueUpdate.Id);

            if(league == null)
            {
                return NotFound();
            }

            var updatedLeague = await _leagueService.UpdateLeague(leagueUpdate, league);

            return Ok(updatedLeague);
        }

        [HttpPatch("join")]
        public async Task<ActionResult<League>> JoinLeague(string leagueId)
        {
            var league = await _leagueService.GetLeagueDetails(leagueId);
            var userId = GetUserId();

            if(league == null)
            {
                return NotFound();
            }

            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if(league.Players.Contains(userId))
            {
                return Conflict("You have already joined this league.");
            }

            var updatedLeague = await _leagueService.JoinLeague(userId, league);

            return Ok(updatedLeague);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> RemoveLeague(string leagueId)
        {
            var userId = GetUserId();
            var league = await _leagueService.GetLeagueDetails(userId);

            if(league == null)
            {
                return NotFound();
            }

            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if(league.OwnerId == userId)
            {
                var result = await _leagueService.RemoveLeague(leagueId);

                return result;
            }

            return Unauthorized();
        }

        private string GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return userId != null ? userId : string.Empty;
        }
    }
}
