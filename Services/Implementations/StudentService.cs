using CourseEnrollment.Data;
using CourseEnrollment.Models.DTO;
using CourseEnrollment.Models.ManyToMany;
using CourseEnrollment.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Service.Implementations;

public class StudentService
{
    private readonly IStudentRepo _studentRepo;
    private readonly AppDbContext _context;
    public StudentService(IStudentRepo studentRepo, AppDbContext context)
    {
        this._studentRepo = studentRepo;
        this._context = context;
    }
    public async Task<IEnumerable<StudentDetailDto>?> GetAllStudentsWithEnrolledCourses()
    {
        var student = await _studentRepo.GetAllStudentsWithEnrolledCourses();
        if (student is null) return null;
        var studentDtos = student.Select(s => new StudentDetailDto
        {
            StudentId = s.Id,
            StudentName = s.StudentName,
            lastUpdated = s.LastUpdated,
            EnrolledCourses = s.Enrollments?.Select(e => new CourseListDto
            {
                Id = e.Course.Id,
                CourseName = e.Course.CourseName,
                EnrolledOn = e.EnrolledOn
            }).ToList()
        }).ToList();
        return studentDtos;
    }
    public async Task<StudentDetailDto?> GetStudentByIdWithEnrolledCourses(Guid id)
    {
        var student = await _studentRepo.GetStudentByIdWithEnrolledCourses(id);
        if (student is null) return null;
        var studentDto = new StudentDetailDto
        {
            StudentId = student.Id,
            StudentName = student.StudentName,
            lastUpdated = student.LastUpdated,
            EnrolledCourses = student.Enrollments?.Select(e => new CourseListDto
            {
                CourseName = e.Course.CourseName,
                EnrolledOn = e.EnrolledOn
            }).ToList()
        };
        return studentDto;
    }
    public async Task<StudentDetailDto?> UpdateStudent(UpdateStudentDto updateStudentDto)
    {
        // here we need null check for the incoming dto
        if (updateStudentDto is null) return null;
        var studentToUpdate = new Student
        {
            Id = updateStudentDto.StudentId,
            StudentName = updateStudentDto.StudentName,
            LastUpdated = updateStudentDto.LastUpdated
        };
        var updatedStudent = await _studentRepo.Update(studentToUpdate);
        if (updatedStudent is null) return null;
        var updatedStudentDto = new StudentDetailDto
        {
            StudentId = updatedStudent.Id,
            StudentName = updatedStudent.StudentName,
            lastUpdated = updatedStudent.LastUpdated,
            EnrolledCourses = []
        };
        return updatedStudentDto;
    }
    public async Task<StudentDetailDto?> CreateStudent(UpdateStudentDto createStudentDto)
    {
        if (createStudentDto is null) return null;
        var student = new Student
        {
            Id = Guid.NewGuid(),
            StudentName = createStudentDto.StudentName,
            LastUpdated = createStudentDto.LastUpdated
        };
        var createdStudent = await _studentRepo.Create(student);
        var createdStudentDto = new StudentDetailDto
        {
            StudentId = createdStudent.Id,
            StudentName = createdStudent.StudentName,
            lastUpdated = createdStudent.LastUpdated,
            EnrolledCourses = []
        };
        return createdStudentDto;
    }
    public async Task<bool> DeleteStudent(Guid studentId)
    {
        return await _studentRepo.Delete(id: studentId);
    }

    public async Task<EnrollmentResult> CancelEnroll(Guid studentId, Guid courseId)
    {
        if (!await _context.Students.AnyAsync(s => s.Id == studentId))
            return EnrollmentResult.StudentNotFound;
        if (!await _context.Courses.AnyAsync(c => c.Id == courseId))
            return EnrollmentResult.CourseNotFound;

        if (await _studentRepo.CancleEnrolledCourse(studentId, courseId))
            return EnrollmentResult.Success;
            return EnrollmentResult.AlreadyEnrolled;
    }
    public async Task<EnrollmentResult> EnrollToCourse(Guid studentId,Guid courseId)
    {
        if ( !await _context.Students.AnyAsync(s => s.Id == studentId))
            return EnrollmentResult.StudentNotFound;
        if ( !await _context.Courses.AnyAsync(c => c.Id == courseId))
            return EnrollmentResult.CourseNotFound;

        var newEnrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledOn = DateTimeOffset.UtcNow
        };
            
        if (await _studentRepo.EnrollToCourse(newEnrollment))
            return EnrollmentResult.Success;
        return EnrollmentResult.AlreadyEnrolled; 
    }
}