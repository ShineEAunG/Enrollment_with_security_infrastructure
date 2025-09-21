using CourseEnrollment.Models.ManyToMany;
using CourseEnrollment.Repository.Interfaces;

namespace CourseEnrollment.Service.Interfaces;


public interface IEmployeeRepo : IGenericRepo<Employee>
{

    Task<Employee?> GetEmployeeByEmail(string Email);
    Task<bool> AddEmployeeToRoleAsync(string employeeId, string roleName);

    Task UpdateEmployee(Employee employee);
    Task SaveChangesAsync();
}