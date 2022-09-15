using EmptyCourseFinderAPI.Models;

namespace EmptyCourseFinderAPI.Interfaces
{
    public interface ILeagueService
    {
        public Task<League?> GetLeagueDetails(string leagueId);
        public Task<League> CreateLeague(string name, string description, string time, string location, bool open, string ownerId);
        public Task<League> JoinLeague(string userId, League league);
        public Task<League> UpdateLeague(LeagueUpdateRequest leagueUpdate, League oldLeague);
        public Task<bool> RemoveLeague(string leagueId);

        public Task<IList<League>> GetAllLeagues();
    }
}
