using Microsoft.Extensions.Options;
using MongoDB.Driver;
using BlogApp.Models;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<UserProfile> _userProfiles;

        public MongoDBService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDbConnection");
            var databaseName = configuration.GetValue<string>("MongoDBSettings:DatabaseName");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string cannot be null.");
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _userProfiles = database.GetCollection<UserProfile>("UserProfiles");
        }

        public async Task<UserProfile> GetUserProfileByEmailAsync(string email)
        {
            return await _userProfiles.Find(user => user.Email == email).FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateUserProfileAsync(string email, UserProfile updatedProfile)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.Email, email);
            var update = Builders<UserProfile>.Update
                .Set(u => u.Username, updatedProfile.Username)
                .Set(u => u.Bio, updatedProfile.Bio)
                .Set(u => u.Location, updatedProfile.Location)
                .Set(u => u.PhoneNumber, updatedProfile.PhoneNumber);

            var result = await _userProfiles.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
        public async Task<bool> FollowUserAsync(string followerEmail, string followingEmail)
        {
            var followerFilter = Builders<UserProfile>.Filter.Eq(u => u.Email, followerEmail);
            var followingFilter = Builders<UserProfile>.Filter.Eq(u => u.Email, followingEmail);

            var followerUpdate = Builders<UserProfile>.Update.AddToSet(u => u.Following, followingEmail);
            var followingUpdate = Builders<UserProfile>.Update.AddToSet(u => u.Followers, followerEmail);

            var followerResult = await _userProfiles.UpdateOneAsync(followerFilter, followerUpdate);
            var followingResult = await _userProfiles.UpdateOneAsync(followingFilter, followingUpdate);

            return followerResult.ModifiedCount > 0 && followingResult.ModifiedCount > 0;
        }

        public async Task<bool> UnfollowUserAsync(string followerEmail, string followingEmail)
        {
            var followerFilter = Builders<UserProfile>.Filter.Eq(u => u.Email, followerEmail);
            var followingFilter = Builders<UserProfile>.Filter.Eq(u => u.Email, followingEmail);

            var followerUpdate = Builders<UserProfile>.Update.Pull(u => u.Following, followingEmail);
            var followingUpdate = Builders<UserProfile>.Update.Pull(u => u.Followers, followerEmail);

            var followerResult = await _userProfiles.UpdateOneAsync(followerFilter, followerUpdate);
            var followingResult = await _userProfiles.UpdateOneAsync(followingFilter, followingUpdate);

            return followerResult.ModifiedCount > 0 && followingResult.ModifiedCount > 0;
        }


        public async Task InsertUserProfileAsync(UserProfile userProfile)
        {
            // Set default values for empty fields

            if (string.IsNullOrWhiteSpace(userProfile.Bio))
            {
                userProfile.Bio = null;  // Set to null if empty
            }

            if (userProfile.Followers == null || userProfile.Followers.Count == 0)
            {
                userProfile.Followers = new List<string>();  // Initialize as an empty list
            }

            if (userProfile.Following == null || userProfile.Following.Count == 0)
            {
                userProfile.Following = new List<string>();  // Initialize as an empty list
            }

            if (string.IsNullOrWhiteSpace(userProfile.PhoneNumber))
            {
                userProfile.PhoneNumber = null;  // Set to null if empty
            }



            await _userProfiles.InsertOneAsync(userProfile);
        }
    }
}
