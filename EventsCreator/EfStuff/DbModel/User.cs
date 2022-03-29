using System.Text.Json.Serialization;

namespace EventsCreator.EfStuff.DbModel
{
    public class User
    {
        public long Id { get; set; }
        public string Login { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string Info { get; set; }
        [JsonIgnore]
        public List<Event> CreatedEvents { get; set; }
        [JsonIgnore]
        public List<Event> InvitedEvents { get; set; }

    }
}
