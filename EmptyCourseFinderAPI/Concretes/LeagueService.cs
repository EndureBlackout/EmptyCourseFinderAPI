using EmptyCourseFinderAPI.Interfaces;
using EmptyCourseFinderAPI.Models;
using EmptyCourseFinderAPI.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EmptyCourseFinderAPI.Concretes
{
    public class LeagueService : ILeagueService
    {
        private readonly MongoSettings _mongoOptions;
        private readonly MongoClient _mongoClient;
        private readonly IMongoCollection<League> _leagues;

        public LeagueService(IOptions<MongoSettings> mongoOptions)
        {
            _mongoOptions = mongoOptions.Value;
            _mongoClient = new MongoClient(_mongoOptions.ConnectionString);
            _leagues = GetLeagueCollection();
        }

        public async Task<IList<League>> GetAllLeagues()
        {
            var result = await _leagues.FindAsync(x => true);

            return await result.ToListAsync();
        }

        public async Task<League?> GetLeagueDetails(string leagueId)
        {
            var result = await _leagues.FindAsync(x => x.Id == leagueId);

            var league = await result.FirstOrDefaultAsync();

            return league;
        }

        public async Task<League?> GetLeagueByOwnerName(string ownerId, string name)
        {
            var result = await _leagues.FindAsync(x => x.OwnerId == ownerId && x.LeagueName == name);

            var league = await result.FirstOrDefaultAsync();

            return league;
        }

        public async Task<League> CreateLeague(string name, string description, string time, string location, bool open, string ownerId)
        {
            var league = new League
            {
                LeagueName = name,
                Description = description,
                LeagueTime = time,
                Location = location,
                OwnerId = ownerId,
                Players = new List<string>(),
                Open = open
            };

            await _leagues.InsertOneAsync(league);

            var newLeague = await GetLeagueByOwnerName(ownerId, name);

            return newLeague;
        }

        public async Task<League> JoinLeague(string userId, League league)
        {
            league.Players.Add(userId);

            await _leagues.ReplaceOneAsync(Builders<League>.Filter.Eq("Id", league.Id), league);

            league = await GetLeagueDetails(league.Id);

            return league;
        }

        public async Task<League> UpdateLeague(LeagueUpdateRequest leagueUpdate, League oldLeague)
        {

            var updatedLeague = new League
            {
                Id = oldLeague.Id,
                Description = leagueUpdate.Description != null ? leagueUpdate.Description : oldLeague.Description,
                LeagueName = leagueUpdate.LeagueName != null ? leagueUpdate.LeagueName : oldLeague.LeagueName,
                LeagueTime = leagueUpdate.LeagueTime != null ? leagueUpdate.LeagueTime : oldLeague.LeagueTime,
                Location = leagueUpdate.Location != null ? leagueUpdate.Location : oldLeague.Location,
                Open = leagueUpdate.Open,
                OwnerId = leagueUpdate.OwnerId != null ? leagueUpdate.OwnerId : oldLeague.OwnerId,
                Players = leagueUpdate.Players != null ? leagueUpdate.Players : oldLeague.Players
            };

            await _leagues.ReplaceOneAsync(Builders<League>.Filter.Eq("Id", updatedLeague.Id), updatedLeague);

            var league = await GetLeagueDetails(updatedLeague.Id);

            return league;
        }

        public async Task<bool> RemoveLeague(string leagueId)
        {
            var result = await _leagues.DeleteOneAsync(Builders<League>.Filter.Eq("Id", leagueId));

            return result.DeletedCount > 0 ? true : false;
        }

        private IMongoCollection<League> GetLeagueCollection()
        {
            var database = _mongoClient.GetDatabase(_mongoOptions.Database);

            return database.GetCollection<League>(_mongoOptions.LeagueCollection);
        }
    }
}
