using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserAccount>(options)
{
    public DbSet<UserAddress> Address { get; set; }
    //public DbSet<UserAccount> UserAccount { get; set; }
    //public DbSet<SavedCoursesEntity> SavedCourses { get; set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // definerar sammansatt nyckel
        builder.Entity<SavedCoursesEntity>()
            .HasKey(x => new { x.UserId, x.CourseId });

        builder.Entity<SavedCoursesEntity>()
            .HasOne<UserAccount>()
            .WithMany(x => x.SavedCourses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        //builder.Entity<SavedCoursesEntity>()
        //    .HasOne<CoursesEntity>()
        //    .WithMany(x => x.SavedCourses)
        //    .HasForeignKey(x => x.CourseId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //36:13
    }

}
