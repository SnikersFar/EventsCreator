using EventsCreator.Controllers.Attributes;
using EventsCreator.EfStuff.DbModel;
using EventsCreator.EfStuff.DbModel.Enums;
using EventsCreator.EfStuff.Repository;
using EventsCreator.Helpers;
using EventsCreator.Models;
using EventsCreator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sailora.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EventsCreator.Controllers
{

    [ApiController]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly EventRepository _eventRepository;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        public UserController(UserRepository userRepository, UserService userService, EventRepository eventRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userService = userService;
            _eventRepository = eventRepository;
            _configuration = configuration;
        }
        
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = _userRepository.GetAll().SingleOrDefault(u => u.Login == model.Username);
            if (user != null && user.Password == model.Password)
            {
                var userRole = user.Role;

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                authClaims.Add(new Claim(ClaimTypes.Role, userRole.ToString()));


                var token = CreateToken(authClaims);
                var refreshToken = GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                _userRepository.Save(user);

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
        [HttpPost]

        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = _userRepository.GetAll().SingleOrDefault(u => u.Login == model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            try
            {
                var newUser = new User()
                {
                    Login = user.UserName,
                    Email = user.Email,
                    CreatedEvents = new List<Event>(),
                    Info = "",
                    InvitedEvents = new List<Event>(),
                    Password = model.Password,
                    Role = Role.user


                };
                _userRepository.Save(newUser);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]

        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = _userRepository.GetAll().SingleOrDefault(u => u.Login == model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            try
            {
                var newUser = new User()
                {
                    Login = user.UserName,
                    Email = user.Email,
                    CreatedEvents = new List<Event>(),
                    Info = "",
                    InvitedEvents = new List<Event>(),
                    Password = model.Password,
                    Role = Role.admin


                };
                _userRepository.Save(newUser);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }

            return Ok(new Response { Status = "Success", Message = "Admin created successfully!" });
        }

        [HttpPost]

        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }


            string username = principal.Identity.Name;

            var user =  _userRepository.GetAll().SingleOrDefault(u => u.Login == username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            _userRepository.Save(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });
        }

        [Authorize]
        [HttpPost]

        public async Task<IActionResult> Revoke(string username)
        {
            var user = _userRepository.GetAll().SingleOrDefault(u => u.Login == username);
            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;
             _userRepository.Save(user);

            return NoContent();
        }


        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }

        //[HttpPost]
        //public IActionResult Authenticate(AuthenticateRequest model)
        //{
        //    var response = _userService.Authenticate(model);

        //    if (response == null)
        //        return BadRequest(new { message = "Username or password is incorrect" });

        //    return Ok(response);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Register(UserModel userModel)
        //{
        //    var response = await _userService.Register(userModel);

        //    if (response == null)
        //    {
        //        return BadRequest(new { message = "Didn't register!" });
        //    }

        //    return Ok(response);
        //}


        [Authorize]
        [HttpPost]
        public IActionResult InvitedInEvent(long eventId)
        {
            var Me = (User)HttpContext.Items["User"];

            var Event = _eventRepository.Get(eventId);
            if (Event == null)
            {
                return NotFound();
            }
            if (Event.Participants.Any(u => u.Id == Me.Id))
            {
                return StatusCode(304);
            }
            Event.Participants.Add(Me);
            _eventRepository.Save(Event);


            return Ok();
        }

        [HttpPost]
        [Authorize]
        [UserAccess(Role.admin)]
        public IActionResult CreateEvenet(EventViewModel viewEvent)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(viewEvent);
            }

            var Event = new Event()
            {
                Creator = (User)HttpContext.Items["User"],
                Speaker = viewEvent.Speaker,
                Description = viewEvent.Description,
                EventTime = viewEvent.EventTime,
                NameOfEvent = viewEvent.NameOfEvent,
                Participants = new List<User>(),
            };
            _eventRepository.Save(Event);

            return Ok();

        }
        [HttpDelete]
        [Authorize]
        [UserAccess(Role.admin)]
        public IActionResult DeleteEvenet(long EventId)
        {
            var MyEvent = _eventRepository.Get(EventId);
            if (MyEvent == null)
                return NotFound();

            _eventRepository.Remove(EventId);

            return Ok();

        }
        [HttpPut]
        [Authorize]
        [UserAccess(Role.admin)]
        public IActionResult ChangeEvent(EventViewModel viewEvent)
        {
            if (!ModelState.IsValid || viewEvent.Id <= 0 || _eventRepository.Get(viewEvent.Id) == null)
            {
                return BadRequest(viewEvent);
            }
            var myEvent = _eventRepository.Get(viewEvent.Id);

            myEvent.NameOfEvent = viewEvent.NameOfEvent;
            myEvent.Speaker = viewEvent.Speaker;
            myEvent.EventTime = viewEvent.EventTime;
            myEvent.Description = viewEvent.Description;
            _eventRepository.Save(myEvent);

            return Ok();

        }

    }
}
