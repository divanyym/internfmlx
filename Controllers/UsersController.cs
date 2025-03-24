using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using MvcMovie.Services;

namespace MvcMovie.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _userService;

        public UsersController()
        {
            _userService = new UserService();
        }

        public IActionResult Index(string search, string sortBy)
        {
            var users = _userService.GetUsers();

            // Filtering (Search)
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                                         u.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                             .ToList();
            }

            // Sorting
            users = sortBy switch
            {
                "name" => users.OrderBy(u => u.Name).ToList(),
                "level" => users.OrderBy(u => u.Level).ToList(),
                "gender" => users.OrderBy(u => u.Gender).ToList(),
                _ => users
            };

            return View(users);
        }

        [HttpPost]
        public IActionResult SaveUser(User user)
        {
            _userService.SaveUser(user);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            if (!_userService.DeleteUser(id))
                return NotFound();

            return RedirectToAction("Index");
        }

        [HttpGet("Users/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var user = _userService.GetUsers().FirstOrDefault(u => u.Id == id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost]
        public IActionResult Edit(User updatedUser)
        {
            _userService.UpdateUser(updatedUser);
            TempData["Success"] = "User berhasil diperbarui.";
            return RedirectToAction("Index");
        }
    }
}
