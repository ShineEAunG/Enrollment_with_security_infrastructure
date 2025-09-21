namespace CourseEnrollment.Models.ManyToMany;

public class Course
{
    public Guid Id { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public ICollection<Enrollment> Enrollments { get; set; } = [];
}