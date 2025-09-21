using CourseEnrollment.Models.ManyToMany;

namespace CourseEnrollment.Repository.Interfaces;

public interface IStudentRepo : IGenericRepo<Student>
{
    Task<IEnumerable<Student>?> GetAllStudentsWithEnrolledCourses();
    Task<Student?> GetStudentByIdWithEnrolledCourses(Guid id);
    Task<Student?> Update(Student student);
    Task<bool> CancleEnrolledCourse(Guid studentId, Guid courseId);
    Task<bool> EnrollToCourse(Enrollment newEnrollment);
} 