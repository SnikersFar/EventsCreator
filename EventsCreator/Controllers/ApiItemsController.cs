using EventsCreator.EfStuff.DbModel;
using EventsCreator.EfStuff.Repository;
using System.Web.Http;
namespace EventsCreator.Controllers
{
    public class ApiItemsController : ApiController
    {
        private EventRepository _eventRepository;

        public ApiItemsController(EventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpGet]
        public List<Event> GetAll()
        {
            return _eventRepository.GetAll();
        }

        [HttpGet]
        public Event Get(long eventId)
        {
            return _eventRepository.Get(eventId);
        }

        [HttpGet]
        public List<Event> GetByFilter(int perPage = 50, int page = 1, string searchName = "", string filtColumnName = "NameOfEvent")
        {
            var filterList = _eventRepository.GetByFilter(perPage, page, searchName, filtColumnName);
            return filterList;
        }






    }
}
