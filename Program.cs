using MvcMovie.Services;


var builder = WebApplication.CreateBuilder(args);

// Tambahkan layanan MVC
builder.Services.AddControllersWithViews();


// Menambahkan konfigurasi dari appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole(); // Menampilkan log di console
    logging.AddDebug();   // Menampilkan log di output debug
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

// Konfigurasi route default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);



app.Run();
