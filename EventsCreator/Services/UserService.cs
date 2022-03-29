using EventsCreator.EfStuff.DbModel;
using EventsCreator.EfStuff.Repository;
using EventsCreator.Helpers;
using EventsCreator.Models;
using Microsoft.IdentityModel.Tokens;
using Sailora.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventsCreator.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _userRepository
                .GetAll()
                .FirstOrDefault(x => x.Login == model.Username && x.Password == model.Password);

            if (user == null)
            {
                // todo: need to add logger
                return null;
            }

            var token = _configuration.GenerateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }
        public async Task<AuthenticateResponse> Register(UserModel userModel)
        {
            var user = new User()
            {
                Info = userModel.Info,
                Login = userModel.Username,
                Password = userModel.Password,
                
            };

             _userRepository.Save(user);

            var response = Authenticate(new AuthenticateRequest
            {
                Username = user.Login,
                Password = user.Password
            });

            return response;
        }

    }
}
