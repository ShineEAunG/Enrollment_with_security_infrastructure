using CourseEnrollment.Models.ManyToMany;

namespace CourseEnrollment.Data.Seeding;

public static class StudentSeedData
{
    public static void SeedData(AppDbContext _context)
    {
       
       var students = new List<Student>
        {
            new Student { Id = Guid.NewGuid(), StudentName = "Alice" },
            new Student { Id = Guid.NewGuid(), StudentName = "Bob" },
            new Student { Id = Guid.NewGuid(), StudentName = "Charlie" }
        };
        _context.Students.AddRange(students);
        var courses = new List<Course>
        {
            new Course { Id = Guid.NewGuid(), CourseName = "Mathematics" },
            new Course { Id = Guid.NewGuid(), CourseName = "Physics" },
            new Course { Id = Guid.NewGuid(), CourseName = "Computer Science" }
        };
        _context.Courses.AddRange(courses);
        // i will call this savechanges in program.cs
        _context.SaveChanges();
        var enrollments = new List<Enrollment>
        {
            new() {
                Student = students[0],
                StudentId = students[0].Id,
                Course = courses[0],
                CourseId = courses[0].Id,
                EnrolledOn = DateTimeOffset.Now.AddDays(-10)
            },
            new() {
                Student = students[0],
                StudentId = students[0].Id,
                Course = courses[1],
                CourseId = courses[1].Id,
                EnrolledOn = DateTimeOffset.Now.AddDays(-5)
            },
            new() {
                Student = students[1],
                StudentId = students[1].Id,
                Course = courses[0],
                CourseId = courses[0].Id,
                EnrolledOn = DateTimeOffset.Now.AddDays(-8)
            },
            new() {
                Student = students[2],
                StudentId = students[2].Id,
                Course = courses[2],
                CourseId = courses[2].Id,
                EnrolledOn = DateTimeOffset.Now.AddDays(-2)
            }
        };
        _context.Enrollments.AddRange(enrollments);
        _context.SaveChanges();          
    }
}