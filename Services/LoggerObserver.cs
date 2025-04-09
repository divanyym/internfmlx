using MvcMovie.Models;
using Microsoft.Extensions.Logging;

namespace MvcMovie.Observer
{
    public class LoggerObserver : IUserObserver
    {
        private readonly ILogger<LoggerObserver> _logger;

        public LoggerObserver(ILogger<LoggerObserver> logger)
        {
            _logger = logger;
        }

        public void OnUserChanged(string action, User user)
        {
            _logger.LogInformation($"User {action}: ID={user.Id}, Name={user.Name}, Email={user.Email}");
        }
    }
}
