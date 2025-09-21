using System.Collections.Generic;

namespace CourseEnrollment.Models.ManyToMany;

public class Student
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public DateTimeOffset LastUpdated { get; set; } 
}