using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // Untuk PasswordHasher
using MvcMovie.Models;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Login> _passwordHasher;

    public AccountController(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Login>();
    }

    // GET: Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(Login model)
    {
        if (ModelState.IsValid)
        {
            // Cari data login berdasarkan username di tabel Login
            var login = _context.Logins.SingleOrDefault(l => l.Username == model.Username);

            if (login != null)
            {
                // Verifikasi password yang dimasukkan dengan password yang ada di tabel Login (hashed password)
                var result = _passwordHasher.VerifyHashedPassword(login, login.Password, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Jika password cocok, simpan sesi atau cookie untuk login
                    HttpContext.Session.SetInt32("AdminId", login.Id);

                    // Redirect ke halaman home setelah login berhasil
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Jika password salah
                    ModelState.AddModelError("", "Password salah.");
                }
            }
            else
            {
                // Jika username tidak ditemukan
                ModelState.AddModelError("", "Username tidak ditemukan.");
            }
        }

        return View(model); // Jika login gagal, kembali ke form login
    }
}
