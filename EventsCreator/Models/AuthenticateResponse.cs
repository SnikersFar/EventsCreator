using EventsCreator.EfStuff.DbModel;

namespace EventsCreator.Helpers
{
    public class AuthenticateResponse
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Info { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(User user, string token)
        {
            Id = user.Id;
            Username = user.Login;
            Info = user.Info;
            Token = token;
        }
    }
}