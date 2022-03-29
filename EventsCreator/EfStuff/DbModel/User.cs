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
    }
}
