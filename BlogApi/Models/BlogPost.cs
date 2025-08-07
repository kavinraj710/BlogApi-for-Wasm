using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogApp.Models
{
    public class UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Location { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public List<string> Followers { get; set; } = new List<string>();

        public List<string> Following { get; set; } = new List<string>();
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}
