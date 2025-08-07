using BlogApp.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;


namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly MongoDBService _mongoDbService;

        public UserProfileController(MongoDBService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        // GET api/userprofile/{email}
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserProfileByEmail(string email)
        {
            var userProfile = await _mongoDbService.GetUserProfileByEmailAsync(email);

            if (userProfile == null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            return Ok(userProfile);
        }

        // PUT api/userprofile/{email}
        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUserProfile(string email, [FromBody] UserProfile updatedProfile)
        {
            if (updatedProfile == null || string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Invalid request." });
            }

            var result = await _mongoDbService.UpdateUserProfileAsync(email, updatedProfile);

            if (result)
            {
                return Ok(new { message = "Profile updated successfully." });
            }

            return NotFound(new { message = "User profile not found." });
        }
        [HttpPost("Follow")]
        public async Task<IActionResult> FollowUser([FromBody] FollowRequest request)
        {
            if (string.IsNullOrEmpty(request.FollowerEmail) || string.IsNullOrEmpty(request.FollowingEmail))
            {
                return BadRequest("Invalid request. Both emails are required.");
            }

            var result = await _mongoDbService.FollowUserAsync(request.FollowerEmail, request.FollowingEmail);

            if (result)
            {
                return Ok("Followed successfully.");
            }
            return BadRequest("Failed to follow user.");
        }

        public class FollowRequest
        {
            public string FollowerEmail { get; set; }
            public string FollowingEmail { get; set; }
        }
        [HttpPost("Unfollow")]
        public async Task<IActionResult> UnfollowUser([FromBody] FollowRequest request)
        {
            if (string.IsNullOrEmpty(request.FollowerEmail) || string.IsNullOrEmpty(request.FollowingEmail))
            {
                return BadRequest("Invalid request. Both emails are required.");
            }

            var result = await _mongoDbService.UnfollowUserAsync(request.FollowerEmail, request.FollowingEmail);

            if (result)
            {
                return Ok("Unfollowed successfully.");
            }
            return BadRequest("Failed to unfollow user.");
        }

        // POST api/userprofile/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserProfile userProfile)
        {
            if (userProfile == null)
            {
                return BadRequest("Request body is empty.");
            }

            Console.WriteLine($"Received user profile: {JsonSerializer.Serialize(userProfile)}");

            if (string.IsNullOrEmpty(userProfile.Email) || string.IsNullOrEmpty(userProfile.Username))
            {
                return BadRequest("Email and Username are required.");
            }

            await _mongoDbService.InsertUserProfileAsync(userProfile);
            return Ok("User registered successfully.");
        }

    }
}