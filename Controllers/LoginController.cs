using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
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
            // Cari data login berdasarkan username
            var user = _context.Logins.SingleOrDefault(l => l.Username == model.Username);

            if (user != null)
            {
                // Verifikasi password plaintext
                if (user.Password == model.Password)
                {
                    // Jika password cocok, simpan session dan redirect ke halaman utama
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    return RedirectToAction("Index", "Home");  
                }
                else
                {
                    ModelState.AddModelError("", "Invalid password.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Username not found.");
            }
        }

        return View(model);
    }
}
