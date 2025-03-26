using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using MvcMovie.Services;

public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    private readonly ILogger<UserService> _userServiceLogger;

    public UsersController(ILogger<UsersController> logger, ILogger<UserService> userServiceLogger)
    {
        _logger = logger;
        _userServiceLogger = userServiceLogger;
    }

    public IActionResult Index(string search, string sortBy, int page = 1, int pageSize = 10)
    {
        _logger.LogInformation("Index method called.");
        
        using (var userService = new UserService(_userServiceLogger))
        {
            var users = userService.GetFilteredUsers(search, sortBy, page, pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)userService.GetUsers().Count() / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.SortBy = sortBy;

            return View(users);
        } // Saat keluar dari `using`, logger akan mencatat bahwa `Dispose()` dipanggil.
    }

    [HttpPost]
    public IActionResult SaveUser(User user)
    {

        _logger.LogInformation($"Saving user: Name={user.Name}, Email={user.Email}");
        
        using (var userService = new UserService(_userServiceLogger))
        {
            userService.SaveUser(user);
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public IActionResult DeleteUser(int id)
    {
        _logger.LogInformation($"DeleteUser sucsessfor ID: {id}");
        
        using (var userService = new UserService(_userServiceLogger))
        {
            if (!userService.DeleteUser(id))
            {
                _logger.LogWarning($"Failed to delete user with ID: {id}");
                return NotFound();
            }
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        _logger.LogInformation($"Get edit called for ID: {id}");
        
        using (var userService = new UserService(_userServiceLogger))
        {
            var user = userService.GetUsers().FirstOrDefault(u => u.Id == id);
            return user == null ? NotFound() : View(user);
        }
    }

    [HttpPost("Edit")]
    public IActionResult Edit(User updatedUser)
    {
        _logger.LogInformation($"Edit Sucsess for ID: {updatedUser.Id}");
        
        using (var userService = new UserService(_userServiceLogger))
        {
            userService.UpdateUser(updatedUser);
            TempData["Success"] = "User berhasil diperbarui.";
            return RedirectToAction("Index");
        }
    }
}
