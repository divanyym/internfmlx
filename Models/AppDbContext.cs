using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Menambahkan konfigurasi untuk kolom Id agar auto-generate
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();  // Mengatur agar ID otomatis di-generate
        }
    
    public DbSet<EmployeeLog> EmployeeLogs{ get; set; }

}
