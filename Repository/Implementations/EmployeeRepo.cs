using CourseEnrollment.Data;
using CourseEnrollment.Models.ManyToMany;
using CourseEnrollment.Repository.Interfaces;
using CourseEnrollment.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Service.Implementations;

public class EmployeeRepo : GenericRepo<Employee>, IEmployeeRepo
{
    public EmployeeRepo(AppDbContext context) : base(context)
    {

    }
    public async Task<Employee?> GetEmployeeByEmail(string email)
    {
        return await _context.Employees
            .Include(e => e.EmployeeRoles)
                .ThenInclude(er => er.Role)
            .FirstOrDefaultAsync(e => e.Email == email);
    }


    public Task UpdateEmployee(Employee employee)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddEmployeeToRoleAsync(string employeeId, string roleName)
    {
        var employeeExist = await _context.Employees.AnyAsync(e => e.EmployeeId == employeeId);
        if (!employeeExist)
            return false;
            
        //check with id it is faster than checking with name
        var roleId = await _context.Roles
            .Where(r => r.RoleName == roleName)
            .Select(r => r.RoleId)
            .FirstOrDefaultAsync();
        if (roleId is null)
            return false;

        
        var employeeInRoleAlready = await _context.EmployeeRoles
            .AnyAsync(er => er.EmployeeId == employeeId && er.Role.RoleId == roleId);  
            
        if (employeeInRoleAlready)
            return true;
        var employeeRole = new EmployeeRole
        {
            EmployeeId = employeeId,
            RoleId = roleId
        };
        _context.EmployeeRoles.Add(employeeRole);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}