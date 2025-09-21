using CourseEnrollment.Data;
using CourseEnrollment.Models.ManyToMany;
using CourseEnrollment.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Repository.Implementation;

public class StudentRepo : GenericRepo<Student>, IStudentRepo
{
    public StudentRepo(AppDbContext context) : base(context)
    {

    }

    public async Task<bool> CancleEnrolledCourse(Guid studentId, Guid courseId)
    {
        var EnrolledCourse = await _context.Enrollments.FirstOrDefaultAsync(e => (e.StudentId == studentId && e.CourseId == courseId));
        if (EnrolledCourse is null) return false;
        _context.Enrollments.Remove(EnrolledCourse);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EnrollToCourse(Enrollment newEnrollment)
    {
        _context.Enrollments.Add(newEnrollment);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<Student>?> GetAllStudentsWithEnrolledCourses()
    {
        var students = await _context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .ToListAsync();
        return students;
    }

    public async Task<Student?> GetStudentByIdWithEnrolledCourses(Guid id)
    {
        if (id == Guid.Empty) return null;
        var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);
        return student;
    }

    // this method hits db with 2 queries
    public async Task<Student?> Update1(Student student)
    {
        var studentToUpdate = await set.FindAsync(student.Id);
        if (studentToUpdate is null)
            return null;
        if (student.LastUpdated != studentToUpdate.LastUpdated)
            return null;
        // CurrentValue is in SET clause
        //_context.Entry(studentToUpdate).CurrentValues.SetValues(student);   // the whole object is updated
        studentToUpdate.StudentName = student.StudentName; 
        // update all the others
        await _context.SaveChangesAsync();
        return studentToUpdate;
    }
    // this hits with only 1 query
    public async Task<Student?> Update(Student student)
    {
        //_context.Entry(obj) // used to attach a disconnected entity to the context
        // OriginalValue is used for WHERE clause
        // CurrentValue is in SET clause
        _context.Entry(student).Property(s => s.LastUpdated).OriginalValue = student.LastUpdated;

        student.LastUpdated = DateTimeOffset.UtcNow;
        try
        {
            await _context.SaveChangesAsync();
            return student;
        }
        catch (DbUpdateConcurrencyException)
        {
            // No rows affected → concurrency conflict
            return null;
        }
    }
}

    //  DUMB ME
    // public async Task<Student?> Update(Student student)
    // {
    //     var studentToUpdate = await set.FindAsync(student.Id);
    //     if (studentToUpdate is null)
    //         return null;
    //     // assign the fetched obj with requested properties
    //     if (student.lastUpdated != studentToUpdate.lastUpdated)
    //     {
    //         return null;
    //     }
    //     studentToUpdate.StudentName = student.StudentName;
    //     //studentToUpdate.Enrollments = student.Enrollments;//this is not good practice, better to update enrollments separately
    //     var enrollmentsInDb = await _context.Enrollments.Where(e => e.StudentId == student.Id).ToListAsync();
    //     foreach (var existing in enrollmentsInDb)
    //     {
    //         //check if the existing enrollment still exists in the incoming enrollments
    //         if (!student.Enrollments.Any(e => e.CourseId == existing.CourseId))
    //         {
    //             _context.Enrollments.Remove(existing);
    //         }
    //     }
    //     foreach (var incoming in student.Enrollments)
    //     {
    //         //check if the incoming enrollment already exists in the database
    //         var existing = enrollmentsInDb.FirstOrDefault(e => e.CourseId == incoming.CourseId);
    //         //if null add new enrollment
    //         if (existing is null)
    //         {
    //             _context.Enrollments.Add(new Enrollment
    //             {
    //                 StudentId = student.Id,
    //                 CourseId = incoming.CourseId,
    //                 EnrolledOn = incoming.EnrolledOn
    //             });
    //         }
    //         //if not null, update the properties
    //         else
    //         {
    //             existing.EnrolledOn = incoming.EnrolledOn;
    //         }
    //     }
    //     studentToUpdate.lastUpdated = DateTimeOffset.UtcNow;

    //         await _context.SaveChangesAsync();
    //         return studentToUpdate;
    // }