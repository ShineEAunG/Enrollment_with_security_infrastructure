

using CourseEnrollment.Data.Configurations;
using CourseEnrollment.Models.Authentications;
using CourseEnrollment.Models.ManyToMany;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {

        }
        // this is not table name we can set it in config
        // in config we can set TblInvoiceItems and still call _context.InvoiceItems
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<EmployeeRole> EmployeeRoles => Set<EmployeeRole>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
            modelBuilder.ApplyConfiguration(new StuentConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeRoleConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration()); 
            base.OnModelCreating(modelBuilder);
        }
    }
}