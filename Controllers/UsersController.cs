using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using MvcMovie.Services;
using System.Linq;

namespace MvcMovie.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _userService;

        public UsersController()
        {
            _userService = new UserService();
        }

        public IActionResult Index(string search, string sortBy, int page = 1, int pageSize = 10)
        {
            var users = _userService.GetUsers();

            // Filtering (Search)
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => (u.Name ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         (u.Email ?? "").Contains(search, StringComparison.OrdinalIgnoreCase))
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

            // Pagination
            int totalUsers = users.Count();
            var paginatedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass data ke View
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.SortBy = sortBy;

            return View(paginatedUsers);
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

        [HttpGet] // ✅ Perbaikan: Tidak perlu URL khusus di sini
        public IActionResult Edit(int id)
        {
            var user = _userService.GetUsers().FirstOrDefault(u => u.Id == id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost("Edit")] // ✅ Perbaikan: Tambahkan ini untuk membedakan dari metode GET
        public IActionResult Edit(User updatedUser)
        {
            _userService.UpdateUser(updatedUser);
            TempData["Success"] = "User berhasil diperbarui.";
            return RedirectToAction("Index");
        }
    }
}
