using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using MvcMovie.Observer;
using MvcMovie.Services;

namespace MvcMovie.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ILogger<UserService> _userServiceLogger;
        private readonly ILogger<LoggerObserver> _observerLogger;
        private readonly AppDbContext _context;

        public UsersController(
            ILogger<UsersController> logger,
            ILogger<UserService> userServiceLogger,
            ILogger<LoggerObserver> observerLogger,
            AppDbContext context)
        {
            _logger = logger;
            _userServiceLogger = userServiceLogger;
            _observerLogger = observerLogger;
            _context = context;
        }

        public IActionResult Index(string search, string sortBy = "name", int page = 1, int pageSize = 10)
        {
        var usersQuery = _context.Users
            .Where(u => string.IsNullOrEmpty(search) || u.Name.Contains(search))
            .OrderBy(u => u.Id)  // Sortir berdasarkan ID untuk urutan yang benar
            .AsQueryable();

        int totalUsers = usersQuery.Count();
        int totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

        ViewBag.TotalPages = totalPages;  // Total halaman
        ViewBag.CurrentPage = page;      // Halaman yang sedang aktif
        ViewBag.PageSize = pageSize;
        ViewBag.Search = search;
        ViewBag.SortBy = sortBy;

        var users = usersQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();  // Ambil data sesuai halaman

        return View(users);
        }


        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var subject = new UserSubject();
            var observer = new LoggerObserver(_observerLogger);
            subject.Attach(observer);

            using var userService = new UserService(_userServiceLogger, subject, _context);
            userService.SaveUser(user);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var subject = new UserSubject();
            var observer = new LoggerObserver(_observerLogger);
            subject.Attach(observer);

            using var userService = new UserService(_userServiceLogger, subject, _context);
            userService.UpdateUser(user);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var subject = new UserSubject();
            var observer = new LoggerObserver(_observerLogger);
            subject.Attach(observer);

            using var userService = new UserService(_userServiceLogger, subject, _context);
            var success = userService.DeleteUser(id);
            if (!success)
                return NotFound();

            return RedirectToAction("Index");
        }
    }
}
