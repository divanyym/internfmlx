using MvcMovie.Services;
using MvcMovie.Observer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Konfigurasi dari appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Tambahkan layanan MVC
builder.Services.AddControllersWithViews();

// Tambahkan service Payroll
builder.Services.AddScoped<IPayrollService, PayrollService>();

// Tambahkan Subject & Observer untuk Observer Pattern
builder.Services.AddSingleton<UserSubject>(); // Singleton supaya observer tetap hidup selama aplikasi jalan
builder.Services.AddScoped<IUserObserver, LoggerObserver>(); // Logger sebagai observer utama

// Tambahkan UserService, injeksikan dependency di constructor
builder.Services.AddScoped<UserService>();

// Konfigurasi logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] "; // Format waktu yang kamu mau
        options.IncludeScopes = false;
        options.SingleLine = true;
    });

    logging.AddDebug(); // Tambahan jika kamu juga ingin debug logger
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

app.UseAuthorization();

// Konfigurasi default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
