using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;
using Microsoft.AspNetCore.Identity;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Login> Logins { get; set; }  
    public DbSet<EmployeeLog> EmployeeLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Menambahkan konfigurasi untuk kolom Id agar auto-generate
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();  // Mengatur agar ID otomatis di-generate

        // Menambahkan data admin pertama kali di tabel Login
        modelBuilder.Entity<Login>().HasData(
            new Login
            {
                Id = -1,
                Username = "admin",  // Username untuk login
                Password = "admin123" // Password di-hash
            }
        );
    }
}
