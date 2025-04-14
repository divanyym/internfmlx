using MvcMovie.Services;
using MvcMovie.Observer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Konfigurasi dari appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Tambahkan DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

// Menambahkan session untuk aplikasi
builder.Services.AddDistributedMemoryCache();  // Menyimpan session di memori
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".MyApp.Session"; // Menentukan nama cookie
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Durasi session
    options.Cookie.HttpOnly = true;  // Hanya bisa diakses oleh server
    options.Cookie.IsEssential = true;  // Memastikan cookie esensial
});

builder.Services.AddScoped<IPayrollService, PayrollService>();

// Subject dan Observer untuk Observer Pattern
builder.Services.AddSingleton<UserSubject>();  // Singleton supaya observer tetap hidup selama aplikasi berjalan
builder.Services.AddScoped<IUserObserver, LoggerObserver>();  // Logger sebagai observer utama

builder.Services.AddScoped<UserService>();

// Konfigurasi logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        options.IncludeScopes = false;
        options.SingleLine = true;
    });

    logging.AddDebug();  // Debug logger
});

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware untuk session
app.UseSession();  // Menambahkan middleware session

app.UseAuthorization();

// Konfigurasi route default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Jalankan aplikasi
app.Run();
