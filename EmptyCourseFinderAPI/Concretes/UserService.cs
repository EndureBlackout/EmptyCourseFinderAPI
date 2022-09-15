using EmptyCourseFinderAPI.Interfaces;
using EmptyCourseFinderAPI.Models;
using EmptyCourseFinderAPI.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EmptyCourseFinderAPI.Concretes
{
    public class UserService : IUserService
    {
        public readonly MongoSettings _mongoSettings;
        public readonly MongoClient _mongoClient;
        public readonly IMongoCollection<User> _users;

        public UserService(IOptions<MongoSettings> mongoOptions)
        {
            _mongoSettings = mongoOptions.Value;
            _mongoClient = new MongoClient(_mongoSettings.ConnectionString);
            _users = GetUserCollection();
        }

        public async Task<User?> GetUserDetails(string userId)
        {
            var result = await _users.FindAsync(x => x.UserId == userId);

            var user = await result.FirstOrDefaultAsync();

            return user;
        }

        public async Task<User> CreateUser(string userId, string name, string lat, string lon, string number, int timeStart, int timeEnd)
        {
            var user = new User
            {
                UserId = userId,
                Lat = lat,
                Lon = lon,
                TimeStart = timeStart,
                TimeEnd = timeEnd,
                Number = number,
                Name = name
            };

            await _users.InsertOneAsync(user);

            return await GetUserDetails(userId);
        }

        public async Task<User> UpdateUserDetails(UpdateUserRequest user)
        {
            var toUpdate = new User
            {
                UserId = user.UserId,
                Lat = user.Lat,
                Lon = user.Lon,
                TimeStart = user.TimeStart,
                TimeEnd = user.TimeEnd,
                Number = user.Number,
                Name = user.Name
            };

            var result = await _users.ReplaceOneAsync(Builders<User>.Filter.Eq("UserId", user.UserId), toUpdate);

            return result.ModifiedCount > 0 ? await GetUserDetails(user.UserId) : null;
        }

        public async Task<bool> DeleteUserDetails(string userId)
        {
            var result = await _users.DeleteOneAsync(Builders<User>.Filter.Eq("UserId", userId));

            return (result.DeletedCount > 0);
        }

        private IMongoCollection<User> GetUserCollection()
        {
            var database = _mongoClient.GetDatabase(_mongoSettings.Database);

            return database.GetCollection<User>(_mongoSettings.UserCollection);
        }
    }
}
