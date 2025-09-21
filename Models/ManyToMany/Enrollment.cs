namespace CourseEnrollment.Models.ManyToMany;

public class Enrollment
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public DateTimeOffset EnrolledOn { get; set; }
    
}
