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
