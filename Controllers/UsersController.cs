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

        public IActionResult Index(string search, string sortBy, int page = 1, int pageSize = 10)
        {
            var users = _userService.GetFilteredUsers(search, sortBy, page, pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)_userService.GetUsers().Count() / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.SortBy = sortBy;

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

        [HttpGet] 
        public IActionResult Edit(int id)
        {
            var user = _userService.GetUsers().FirstOrDefault(u => u.Id == id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost("Edit")] 
        public IActionResult Edit(User updatedUser)
        {
            _userService.UpdateUser(updatedUser);
            TempData["Success"] = "User berhasil diperbarui.";
            return RedirectToAction("Index");
        }
    }
}
