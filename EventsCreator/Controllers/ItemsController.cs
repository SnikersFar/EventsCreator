using EventsCreator.EfStuff.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;

namespace EventsCreator.Controllers
{
    public class ItemsController : ApiController
    {
        private EventRepository _eventRepository;

        public ItemsController(EventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
    }
}
