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
        } // When exiting the 'using', the logger will record that 'Dispose()' was called.
    }

    [HttpPost]
    public IActionResult SaveUser(User user)
    {
        _logger.LogInformation($"Saving user: Name={user.Name}, Email={user.Email}");
        
        using (var userService = new UserService(_userServiceLogger))
        {
            try
            {
                userService.SaveUser(user);
                _logger.LogInformation($"User {user.Name} with ID {user.Id} saved successfully.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving user: {user.Name}, Email={user.Email}");
                TempData["Error"] = "There was an error saving the user.";
                return View();
            }
        }
    }

    [HttpGet]
    public IActionResult DeleteUser(int id)
    {
        _logger.LogInformation($"DeleteUser called for ID: {id}");
        
        using (var userService = new UserService(_userServiceLogger))
        {
            if (!userService.DeleteUser(id))
            {
                _logger.LogWarning($"Failed to delete user with ID: {id}");
                return NotFound();
            }

            _logger.LogInformation($"User with ID {id} deleted successfully.");
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
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found for editing.");
                return NotFound();
            }

            return View(user);
        }
    }

    [HttpPost("Edit")]
    public IActionResult Edit(User updatedUser)
    {
        _logger.LogInformation($"Edit success for ID: {updatedUser.Id}");
        
        using (var userService = new UserService(_userServiceLogger))
        {
            try
            {
                userService.UpdateUser(updatedUser);
                _logger.LogInformation($"User {updatedUser.Name} with ID {updatedUser.Id} updated successfully.");
                TempData["Success"] = "User successfully updated.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user ID: {updatedUser.Id}");
                TempData["Error"] = "There was an error updating the user.";
                return View();
            }
        }
    }
}
