using System.ComponentModel.DataAnnotations;

namespace CourseEnrollment.Models.DTO;


// using dto for update operations need to set the original value of the concurrency token
// but already fetched from db, the entity will have the original value of the concurrency token so it is not needed to set the original value again
public class UpdateStudentDto
{
    [Required(ErrorMessage = "StudentId is required")]
    public Guid StudentId { get; set; }
    [Required(ErrorMessage = "Student name is required")]
    public required string StudentName { get; set; }
    [Required(ErrorMessage = "LastUpdated is required")]
    public DateTimeOffset LastUpdated { get; set; }

}
public class UpdateCourseDto
{
    public required string CourseName { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
}
public class UpdateEnrollmentDto
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
}