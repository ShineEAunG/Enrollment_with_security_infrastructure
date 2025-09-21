

using CourseEnrollment.Models.ManyToMany;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseEnrollment.Data.Configurations;

public class StuentConfiguration :IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("TblStudetns");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StudentName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.LastUpdated)
            .HasPrecision(3)    // the c# datetimeoffset precision is 7 by default, we can set it to 3
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");
            //.HasDefaultValueSql("getutcdate()"); this will return datetime but not datetimeoffset
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student);
    }
}