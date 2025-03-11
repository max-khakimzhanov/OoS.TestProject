using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OoS.TestProject.DAL.Entities;
namespace OoS.TestProject.DAL.Persistence
{
    public class OoSTestProjectDbContext : IdentityDbContext<User>
    {
        public OoSTestProjectDbContext(DbContextOptions<OoSTestProjectDbContext> options) 
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Students)
                .WithMany(s => s.Courses);

            // Seeding data
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, Name = "Alice Johnson", Email = "alice.johnson@example.com" },
                new Teacher { Id = 2, Name = "Bob Smith", Email = "bob.smith@example.com" }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Title = "Math 101", Credits = 3, TeacherId = 1 },
                new Course { Id = 2, Title = "History", Credits = 2, TeacherId = 2 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "John Doe", Email = "john.doe@example.com" },
                new Student { Id = 2, Name = "Jane Doe", Email = "jane.doe@example.com" }
            );
        }
    }
}
