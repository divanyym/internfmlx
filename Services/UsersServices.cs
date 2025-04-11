using MvcMovie.Models;
using MvcMovie.Observer;

namespace MvcMovie.Services
{
    public class UserService : IDisposable
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserSubject _subject;
        private readonly AppDbContext _context;

        public UserService(ILogger<UserService> logger, UserSubject subject, AppDbContext context)
        {
            _logger = logger;
            _subject = subject;
            _context = context;
            _logger.LogInformation("UserService instance created.");
        }

        public void Dispose()
        {
            _logger.LogInformation("UserService instance disposed.");
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public IEnumerable<User> GetFilteredUsers(string search, string sortBy, int page, int pageSize)
        {
            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
            }

            users = sortBy switch
            {
                "name" => users.OrderBy(u => u.Name),
                "level" => users.OrderBy(u => u.Level),
                "gender" => users.OrderBy(u => u.Gender),
                _ => users
            };

            return users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public void SaveUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            _subject.Notify("Added", user);
        }

        public bool DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            _context.SaveChanges();
            
            _subject.Notify("Deleted", user);
            return true;
        }

        public void UpdateUser(User updatedUser)
        {
            _context.Users.Update(updatedUser);
            _context.SaveChanges();
            _subject.Notify("Updated", updatedUser);
        }
    }
}
