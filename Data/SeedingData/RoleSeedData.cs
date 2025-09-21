using System.Threading.Tasks;
using CourseEnrollment.Models.ManyToMany;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace CourseEnrollment.Data.Seeding;

public static class RoleSeedData
{
    public static void SeedRoles(AppDbContext _context)
    {
        if (!_context.Roles.Any(r => r.RoleName == StaticEmployeeRole.Admin))
        {
            var adminRole = new Role
            {
                RoleName = StaticEmployeeRole.Admin,
                CreatedBy = CreatedBy.System,
            };
             _context.Roles.Add(adminRole);

            var hrRole = new Role
            {
                RoleName = StaticEmployeeRole.Premium,
                CreatedBy = CreatedBy.System,
            };
            _context.Roles.Add(hrRole);

            var normal = new Role
            {
                RoleName = StaticEmployeeRole.Normal,
                CreatedBy = CreatedBy.System,
            };
            _context.Roles.Add(normal);
             _context.SaveChanges();
        }          
    }
}