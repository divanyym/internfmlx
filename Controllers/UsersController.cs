using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class UsersController : Controller
    {
        private string filePath = "wwwroot/data.csv";

        // Fungsi untuk membaca data dari CSV
        private List<User> ReadCsvData()
        {
            List<User> users = new List<User>();

            if (System.IO.File.Exists(filePath))
            {
                var lines = System.IO.File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1)) // Skip header
                {
                    var values = line.Split(',');
                    users.Add(new User
                    {
                        Id = int.Parse(values[0]),
                        Name = values[1],
                        Level = values[2],
                        Gender = values[3],
                        Address = values[4],
                        Phone = values[5],
                        Email = values[6]
                    });
                }
            }

            return users;
        }

        // Menampilkan data dengan sorting & search
        public IActionResult Index(string search, string sortBy)
        {
        var users = ReadCsvData();

        // Filtering (Search)
        if (!string.IsNullOrEmpty(search))
        {
            users = users.Where(u => u.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                                    u.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                        .ToList();
    }

            // Sorting
            switch (sortBy)
            {
                case "name":
                    users = users.OrderBy(u => u.Name).ToList();
                    break;
                case "level":
                    users = users.OrderBy(u => u.Level).ToList();
                    break;
                case "gender":
                    users = users.OrderBy(u => u.Gender).ToList();
                    break;
                default:
                    break;
            }

            return View(users);
        }

        // Menyimpan data baru ke CSV
        [HttpPost]
        public IActionResult SaveUser(User user)
        {
            var users = ReadCsvData();
            user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1; // ID otomatis

            string newData = $"{user.Id},{user.Name},{user.Level},{user.Gender},{user.Address},{user.Phone},{user.Email}\n";

            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, "Id,Name,Level,Gender,Address,Phone,Email\n");
            }

            System.IO.File.AppendAllText(filePath, newData);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var users = ReadCsvData();
            var updatedUsers = users.Where(u => u.Id != id).ToList(); // Hapus user yang ID-nya sesuai

            // Tulis ulang CSV tanpa user yang dihapus
            List<string> lines = new List<string> { "Id,Name,Level,Gender,Address,Phone,Email" };
            foreach (var user in updatedUsers)
            {
                lines.Add($"{user.Id},{user.Name},{user.Level},{user.Gender},{user.Address},{user.Phone},{user.Email}");
            }

            System.IO.File.WriteAllLines(filePath, lines);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateUser(User updatedUser)
        {
            var users = ReadCsvData();
            var userIndex = users.FindIndex(u => u.Id == updatedUser.Id);

            if (userIndex != -1)
            {
                users[userIndex] = updatedUser; // Ganti data user dengan data baru
            }

            // Tulis ulang CSV dengan data yang diperbarui
            List<string> lines = new List<string> { "Id,Name,Level,Gender,Address,Phone,Email" };
            foreach (var user in users)
            {
                lines.Add($"{user.Id},{user.Name},{user.Level},{user.Gender},{user.Address},{user.Phone},{user.Email}");
            }

            System.IO.File.WriteAllLines(filePath, lines);
            return RedirectToAction("Index");
}


    }
}
