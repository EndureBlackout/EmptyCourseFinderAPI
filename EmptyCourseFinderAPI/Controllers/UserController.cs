using EmptyCourseFinderAPI.Interfaces;
using EmptyCourseFinderAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmptyCourseFinderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        public readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("details")]
        public async Task<ActionResult<User>> GetUserDetails()
        {
            var userID = GetUserId();

            if(string.IsNullOrEmpty(userID))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserDetails(userID);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody]User user)
        {
            var userId = GetUserId();

            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if(await _userService.GetUserDetails(userId) != null)
            {
                return Conflict();
            }

            var createdUser = await _userService.CreateUser(userId, user.Name, user.Lat, user.Lon, user.Number, user.TimeStart, user.TimeEnd);

            if(createdUser == null)
            {
                return StatusCode(500);
            }

            return CreatedAtAction(nameof(CreateUser), createdUser);
        }

        [HttpPatch]
        public async Task<ActionResult<User>> UpdateUserDetails([FromBody]UpdateUserRequest updateRequest)
        {
            var userId = GetUserId();

            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if(await _userService.GetUserDetails(userId) == null)
            {
                var newUser = new User
                {
                    UserId = userId,
                    Name = updateRequest.Name,
                    Lat = updateRequest.Lat,
                    Lon = updateRequest.Lon,
                    Number = updateRequest.Number,
                    TimeStart = updateRequest.TimeStart,
                    TimeEnd = updateRequest.TimeEnd
                };

                return await CreateUser(newUser);
            }

            var updatedUser = await _userService.UpdateUserDetails(updateRequest);

            if(updatedUser == null)
            {
                return StatusCode(500);
            }

            return Ok(updatedUser);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteUserDetails()
        {
            var userId = GetUserId();

            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var deletedUser = await _userService.DeleteUserDetails(userId);

            return deletedUser ? Ok() : StatusCode(500);
        }

        private string GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return userId != null ? userId : string.Empty;
        }
    }
}
