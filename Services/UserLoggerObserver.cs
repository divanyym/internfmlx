using Microsoft.Extensions.Logging;
using MvcMovie.Models;

namespace MvcMovie.Observer
{
    public class UserLoggerObserver : IUserObserver
    {
        private readonly ILogger<UserLoggerObserver> _logger;

        public UserLoggerObserver(ILogger<UserLoggerObserver> logger)
        {
            _logger = logger;
        }

        public void OnUserChanged(string action, User user)
        {
            _logger.LogInformation($"User {action}: {user.Name} (ID: {user.Id})");
        }
    }
}
