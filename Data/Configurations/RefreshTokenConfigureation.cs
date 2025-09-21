

using CourseEnrollment.Models.Authentications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseEnrollment.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("TblRefreshTokens");

        builder.HasOne(f => f.Employee)
            .WithMany()
            .HasForeignKey(f => f.EmployeeId);
    }
}