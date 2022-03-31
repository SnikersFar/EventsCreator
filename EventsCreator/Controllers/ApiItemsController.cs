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

        [HttpPost]
        public void Add(Event model)
        {
            if(model != null && model.Id <= 0)
            {
                _eventRepository.Save(model);
            }
        }

        [HttpPut]
        [Authorize]
        public IHttpActionResult Change(Event model)
        {
            if (model == null || _eventRepository.Get(model.Id) == null || .Id != _eventRepository.Get(model.Id).Creator.Id)
            return NotFound();

            var OldModel = _eventRepository.Get(model.Id);
            
            OldModel.Speaker = model.Speaker;
            OldModel.NameOfEvent = model.NameOfEvent;
            OldModel.Description = model.Description;
            OldModel.EventTime = model.EventTime;

            _eventRepository.Save(OldModel);
            return Ok();
        }
        [HttpDelete]
        public void Delete(long id) => _eventRepository.Remove(id);

    }
}
