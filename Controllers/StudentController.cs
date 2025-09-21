using System.Threading.Tasks;
using CourseEnrollment.Models.DTO;
using CourseEnrollment.Models.ManyToMany;
using CourseEnrollment.Service.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EfCoreBasic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController(StudentService service) : ControllerBase
{
    [Authorize(Roles = $"{StaticEmployeeRole.Normal},{StaticEmployeeRole.Admin},{StaticEmployeeRole.Premium}")] // here it must be const string the static string will give you error
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await service.GetAllStudentsWithEnrolledCourses();
        return Ok(students);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = $"{StaticEmployeeRole.Normal},{StaticEmployeeRole.Admin},{StaticEmployeeRole.Premium}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var student = await service.GetStudentByIdWithEnrolledCourses(id);
        return Ok(student);
    }
    [HttpPost]
    public async Task<IActionResult> Create(UpdateStudentDto studentDto)
    {
        var studentToCreate = await service.CreateStudent(studentDto);
        if (studentToCreate is null) return BadRequest();

        return CreatedAtAction("Get", new { id = studentToCreate.StudentId }, studentToCreate);
    }
    [HttpDelete]
    [Authorize(Roles = $"{StaticEmployeeRole.Admin}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await service.DeleteStudent(id)) return BadRequest();

        return NoContent();
    }
    [HttpPut("id")]
    [Authorize(Roles = $"{StaticEmployeeRole.Admin},{StaticEmployeeRole.Premium}")]
    public async Task<IActionResult> Update(UpdateStudentDto studentDto)
    {
        if (await service.UpdateStudent(studentDto) is null)
            return BadRequest();

        return NoContent();
    }
    [HttpDelete("{studentId}/CancleEnrollments/{courseId}")]
    [Authorize(Roles = $"{StaticEmployeeRole.Admin},{StaticEmployeeRole.Premium}")]
    public async Task<IActionResult> CancelEnrollment(Guid studentId, Guid courseId)
    {
        var result = await service.CancelEnroll(studentId, courseId);
        if (result == EnrollmentResult.Failed)
            return BadRequest();
        if (result == EnrollmentResult.StudentNotFound || result == EnrollmentResult.CourseNotFound)
            return NotFound();
        return NoContent();
    }
    [HttpPost("{studentId}/EnrollCourse/{courseId}")]
    [Authorize(Roles = $"{StaticEmployeeRole.Admin},{StaticEmployeeRole.Premium}")]
    public async Task<IActionResult> AddNewEnrollment(Guid studentId, Guid courseId)
    {
        var result = await service.EnrollToCourse(studentId, courseId);
        if (result == EnrollmentResult.Success)
        {
            return Ok(result);
        }
        else if (result == EnrollmentResult.StudentNotFound)
        {
            return NotFound("Student not found");
        }
        else if (result == EnrollmentResult.CourseNotFound)
        {
            return NotFound("Course not found");
        }
        else if (result == EnrollmentResult.AlreadyEnrolled)
        {
            return Conflict("Student already enrolled to this course");
        }
        return NotFound();
    }
}