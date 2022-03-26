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
        public List<Event> GetAll() => _eventRepository.GetAll();

        [HttpGet]
        public Event Get(long id) => _eventRepository.Get(id);

        [HttpPost]
        public void Add(Event model)
        {
            if(model != null)
            {
                _eventRepository.Save(model);
            }
        }

        [HttpPut]
        public IHttpActionResult Change(Event model)
        {
            if(model == null || _eventRepository.Get(model.Id) == null)
            return NotFound();

            _eventRepository.Save(model);
            return Ok();
        }
        [HttpDelete]
        public void Delete(long id) => _eventRepository.Remove(id);

    }
}
