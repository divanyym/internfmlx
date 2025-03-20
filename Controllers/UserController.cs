using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class UserInternController : Controller
    {
        public IActionResult Index()
        {
            var interns = new List<UserModel>
            {
                new UserModel { Id = 1, Name = "Alice", Email = "alice@example.com", Position = "Frontend Intern" },
                new UserModel { Id = 2, Name = "Bob", Email = "bob@example.com", Position = "Backend Intern" },
                new UserModel { Id = 3, Name = "Charlie", Email = "charlie@example.com", Position = "Data Science Intern" }
            };

            return View(interns);
        }
    }
}
