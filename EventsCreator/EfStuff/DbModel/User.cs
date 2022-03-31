using EventsCreator.EfStuff.DbModel.Enums;
using System.Text.Json.Serialization;

namespace EventsCreator.EfStuff.DbModel
{
    public class User
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string Info { get; set; }
        public Role Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenTime { get; set; }
        [JsonIgnore]
        public virtual List<Event> CreatedEvents { get; set; }
        [JsonIgnore]
        public virtual List<Event> InvitedEvents { get; set; }

    }
}
