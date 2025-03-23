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
                    if (values.Length < 7) continue; // Lewati jika tidak cukup data

                    users.Add(new User
                    {
                        Id = int.TryParse(values[0], out int id) ? id : 0, // Mencegah error parsing ID
                        Name = values[1].Trim('"'),  // Menghilangkan tanda kutip jika ada
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

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            Console.WriteLine($"Delete request for ID: {id}"); // Debugging

            var users = ReadCsvData();
            var userToDelete = users.FirstOrDefault(u => u.Id == id);

            if (userToDelete == null)
            {
                Console.WriteLine("User not found!"); // Debugging
                return NotFound();
            }

            Console.WriteLine($"Deleting user: {userToDelete.Name}"); // Debugging
            var updatedUsers = users.Where(u => u.Id != id).ToList();

            // Tulis ulang CSV tanpa user yang dihapus
            List<string> lines = new List<string> { "Id,Name,Level,Gender,Address,Phone,Email" };
            foreach (var user in updatedUsers)
            {
                lines.Add($"{user.Id},\"{user.Name}\",\"{user.Level}\",\"{user.Gender}\",\"{user.Address}\",\"{user.Phone}\",\"{user.Email}\"");
            }

            System.IO.File.WriteAllLines(filePath, lines);
            return RedirectToAction("Index");
        }

        [HttpGet("Users/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var users = ReadCsvData();
            var user = users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound(); // 404 kalau user tidak ditemukan
            }

            return View(user);
        }

        [HttpPost]
       
        public IActionResult Edit(User updatedUser)

        {
            var users = ReadCsvData();
            var userIndex = users.FindIndex(u => u.Id == updatedUser.Id);

            if (userIndex == -1)
            {
                TempData["Error"] = "User tidak ditemukan.";
                return RedirectToAction("Index");
            }

            // Update data user
            users[userIndex] = updatedUser;

            // Tulis ulang CSV
            List<string> lines = new List<string> { "Id,Name,Level,Gender,Address,Phone,Email" };
            foreach (var user in users)
            {
                lines.Add($"{user.Id},\"{user.Name}\",\"{user.Level}\",\"{user.Gender}\",\"{user.Address}\",\"{user.Phone}\",\"{user.Email}\"");
            }

            System.IO.File.WriteAllLines(filePath, lines);
            
            TempData["Success"] = "User berhasil diperbarui.";
            return RedirectToAction("Index");
        }
    }
}
