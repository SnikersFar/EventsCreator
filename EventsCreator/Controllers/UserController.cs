using EventsCreator.EfStuff.DbModel;
using EventsCreator.EfStuff.Repository;
using EventsCreator.Helpers;
using EventsCreator.Models;
using EventsCreator.Services;
using Microsoft.AspNetCore.Mvc;
using Sailora.Identity.Models;
using System.Security.Claims;

namespace EventsCreator.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly EventRepository _eventRepository;
        private readonly UserService _userService;

        public UserController(UserRepository userRepository, UserService userService, EventRepository eventRepository)
        {
            _userRepository = userRepository;
            _userService = userService;
            _eventRepository = eventRepository;
        }


        [HttpPost]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserModel userModel)
        {
            var response = await _userService.Register(userModel);

            if (response == null)
            {
                return BadRequest(new { message = "Didn't register!" });
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var Me = (User)HttpContext.Items["User"];
            var users = _userRepository.GetAll();
            return Ok(Me);
        }


    }
}
