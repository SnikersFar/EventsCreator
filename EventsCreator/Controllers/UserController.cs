using EventsCreator.Controllers.Attributes;
using EventsCreator.EfStuff.DbModel;
using EventsCreator.EfStuff.DbModel.Enums;
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
        public IActionResult GetEvents()
        {
            var Me = (User)HttpContext.Items["User"];

            return Ok(Me.CreatedEvents);
        }
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

    }
}
