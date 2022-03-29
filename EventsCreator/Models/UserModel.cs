using System.Text.Json.Serialization;

namespace Sailora.Identity.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Info { get; set; }
        public string Password { get; set; }
    }
}