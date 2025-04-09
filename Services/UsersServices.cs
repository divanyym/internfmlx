using MvcMovie.Models;
using MvcMovie.Observer;

namespace MvcMovie.Services
{
    public class UserService : IDisposable
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserSubject _subject;
        private readonly string filePath = "wwwroot/data.csv";

        public UserService(ILogger<UserService> logger, UserSubject subject)
        {
            _logger = logger;
            _subject = subject;
            _logger.LogInformation("UserService instance created.");
        }

        public void Dispose()
        {
            _logger.LogInformation("UserService instance disposed.");
        }

        public IEnumerable<User> GetUsers()
        {
            var users = new List<User>();

            if (System.IO.File.Exists(filePath))
            {
                var lines = System.IO.File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1))
                {
                    var values = line.Split(',');
                    if (values.Length < 7) continue;

                    users.Add(new User
                    {
                        Id = int.TryParse(values[0], out int id) ? id : 0,
                        Name = values[1].Trim('"'),
                        Level = values[2].Trim('"'),
                        Gender = values[3].Trim('"'),
                        Address = values[4].Trim('"'),
                        Phone = values[5].Trim('"'),
                        Email = values[6].Trim('"')
                    });
                }
            }

            return users;
        }

        public IEnumerable<User> GetFilteredUsers(string search, string sortBy, int page, int pageSize)
        {
            var users = GetUsers().ToList();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => (u.Name ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || (u.Email ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            users = sortBy switch
            {
                "name" => users.OrderBy(u => u.Name).ToList(),
                "level" => users.OrderBy(u => u.Level).ToList(),
                "gender" => users.OrderBy(u => u.Gender).ToList(),
                _ => users
            };

            return users.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public void SaveUser(User user)
        {
            var usersList = GetUsers().ToList();
            int newId = usersList.Any() ? usersList.Max(u => u.Id) + 1 : 1;
            user.Id = newId;

            string newData = $"{user.Id},\"{user.Name}\",\"{user.Level}\",\"{user.Gender}\",\"{user.Address}\",\"{user.Phone}\",\"{user.Email}\"\n";

            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, "Id,Name,Level,Gender,Address,Phone,Email\n");
            }

            System.IO.File.AppendAllText(filePath, newData);

            _subject.Notify("Added", user); // Notify observers
        }

        public bool DeleteUser(int id)
        {
            var users = GetUsers();
            var userToDelete = users.FirstOrDefault(u => u.Id == id);

            if (userToDelete == null) return false;

            var updatedUsers = users.Where(u => u.Id != id).ToList();
            WriteUsersToFile(updatedUsers);

            _subject.Notify("Deleted", userToDelete); // Notify observers

            return true;
        }

        public void UpdateUser(User updatedUser)
        {
            var usersList = GetUsers().ToList();
            var userIndex = usersList.FindIndex(u => u.Id == updatedUser.Id);

            if (userIndex != -1)
            {
                usersList[userIndex] = updatedUser;
                WriteUsersToFile(usersList);

                _subject.Notify("Updated", updatedUser); // Notify observers
            }
        }

        private void WriteUsersToFile(List<User> users)
        {
            List<string> lines = new List<string> { "Id,Name,Level,Gender,Address,Phone,Email" };
            foreach (var user in users)
            {
                lines.Add($"{user.Id},\"{user.Name}\",\"{user.Level}\",\"{user.Gender}\",\"{user.Address}\",\"{user.Phone}\",\"{user.Email}\"");
            }

            System.IO.File.WriteAllLines(filePath, lines);
        }
    }
}
