using EventsCreator.EfStuff.DbModel;
using Microsoft.EntityFrameworkCore;

namespace EventsCreator.EfStuff.Repository
{
    public class EventRepository
    {
        protected WebContext _webContext;
        protected DbSet<Event> _dbSet;

        public EventRepository(WebContext webContext)
        {
            _webContext = webContext;
            _dbSet = webContext.Set<Event>();
        }

        public Event Get(long id)
        {
            return _dbSet.SingleOrDefault(x => x.Id == id);
        }

        public List<Event> GetAll()
        {
            return _dbSet.ToList();
        }

        public void Save(Event model)
        {
            if (model.Id > 0)
            {
                _webContext.Update(model);
            }
            else
            {
                _dbSet.Add(model);
            }
            _webContext.SaveChanges();
        }
        public void Remove(long id)
        {
            if(Get(id) != null)
            {
                _dbSet.Remove(Get(id));
                _webContext.SaveChanges();
            } else
            {
                throw new ArgumentNullException("item");
            }
        }
    }
}
