using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using MvcMovie.Observer;
using MvcMovie.Services;

public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    private readonly ILogger<UserService> _userServiceLogger;
    private readonly ILogger<LoggerObserver> _observerLogger;

    public UsersController(ILogger<UsersController> logger, ILogger<UserService> userServiceLogger, ILogger<LoggerObserver> observerLogger)
    {
        _logger = logger;
        _userServiceLogger = userServiceLogger;
        _observerLogger = observerLogger;
    }

    public IActionResult Index(string search, string sortBy, int page = 1, int pageSize = 10)
    {
        var subject = new UserSubject();
        subject.Attach(new LoggerObserver(_observerLogger));

        using var userService = new UserService(_userServiceLogger, subject);

        var users = userService.GetFilteredUsers(search, sortBy, page, pageSize).ToList();

        ViewBag.TotalPages = (int)Math.Ceiling((double)userService.GetUsers().Count() / pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.Search = search;
        ViewBag.SortBy = sortBy;

        return View(users);
    }

    [HttpPost]
    public IActionResult SaveUser(User user)
    {
        var subject = new UserSubject();
        subject.Attach(new LoggerObserver(_observerLogger));

        using var userService = new UserService(_userServiceLogger, subject);

        try
        {
            userService.SaveUser(user);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving user: {user.Name}");
            TempData["Error"] = "There was an error saving the user.";
            return View();
        }
    }

    [HttpGet]
    public IActionResult DeleteUser(int id)
    {
        var subject = new UserSubject();
        subject.Attach(new LoggerObserver(_observerLogger));

        using var userService = new UserService(_userServiceLogger, subject);

        if (!userService.DeleteUser(id))
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var subject = new UserSubject();
        using var userService = new UserService(_userServiceLogger, subject);

        var user = userService.GetUsers().FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost("Edit")]
    public IActionResult Edit(User updatedUser)
    {
        var subject = new UserSubject();
        subject.Attach(new LoggerObserver(_observerLogger));

        using var userService = new UserService(_userServiceLogger, subject);

        try
        {
            userService.UpdateUser(updatedUser);
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
