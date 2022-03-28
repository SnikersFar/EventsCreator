using EventsCreator.EfStuff.DbModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
        public List<Event>GetByFilter(int perPage, int page, string searchName, string filtColumnName)
        {
            List<Event> filtList;
            Type TypeEvent = typeof(Event);
            if (TypeEvent.GetProperty("filtColumnName") != null)
            {
                var table = Expression.Parameter(typeof(Event), "event");
                var ListOfProperty = filtColumnName.Split(".");
                var member = Expression.Property(table, ListOfProperty[0]);
                for (int i = 1; i < ListOfProperty.Length; i++)
                {
                    var item = ListOfProperty[i];
                    var next = Expression.Property(member, item);
                    member = next;

                }
                var condition = Expression.Lambda<Func<Event, object>>(Expression.Convert(member, typeof(object)), table);

                filtList = _dbSet.OrderBy(condition).ToList();
            } else
            {
                filtList = _dbSet.ToList();
            }
            
            filtList = filtList.Where(e => e.NameOfEvent.Contains(searchName)).ToList();
            filtList = GetForPagination(filtList, perPage, page);


            return filtList;
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
            if (Get(id) != null)
            {
                _dbSet.Remove(Get(id));
                _webContext.SaveChanges();
            }
        }
        public List<Event> GetForPagination(int perPage, int page)
        => _dbSet
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToList();

        private List<Event> GetForPagination(List<Event> events, int perPage, int page)
            => events
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToList();
    }
}
