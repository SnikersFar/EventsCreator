using EventsCreator.EfStuff.DbModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EventsCreator.EfStuff.Repository
{
    public class UserRepository
    {
        protected WebContext _webContext;
        protected DbSet<User> _dbSet;

        public UserRepository(WebContext webContext)
        {
            _webContext = webContext;
            _dbSet = webContext.Set<User>();
        }

        public User Get(long id)
        {
            return _dbSet.SingleOrDefault(x => x.Id == id);
        }

        public List<User> GetAll()
        {
            return _dbSet.ToList();
        }
       
        public void Save(User model)
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
     
    }
}
