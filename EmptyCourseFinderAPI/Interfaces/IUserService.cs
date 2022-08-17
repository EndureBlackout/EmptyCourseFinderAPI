using EmptyCourseFinderAPI.Models;

namespace EmptyCourseFinderAPI.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetUserDetails(string userId);
        public Task<User> CreateUser(string userId, string name, string lat, string lon, string number, int timeStart, int timeEnd);
        public Task<User> UpdateUserDetails(UpdateUserRequest user);

        public Task<bool> DeleteUserDetails(string userId);
    }
}
