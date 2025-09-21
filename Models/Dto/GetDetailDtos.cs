namespace CourseEnrollment.Models.DTO;

public class StudentDetailDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public List<CourseListDto>? EnrolledCourses { get; set; }
    public DateTimeOffset lastUpdated { get; set; } 
 }

public class CourseListDto {
    public Guid Id{ get; set; }
    public required string CourseName { get; set; }
    public DateTimeOffset EnrolledOn { get; set; }
 }
public class CourseDetailDto
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public List<StudentListDto>? Enrollments { get; set; }
    public DateTimeOffset lastUpdated { get; set; }
 }
public class StudentListDto
{
   public Guid StudentId { get; set; }
   public string StudentName { get; set; } = null!;
}

public class EnrollmentDetailDto
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    
    public DateTimeOffset EnrolledOn { get; set; }
}
