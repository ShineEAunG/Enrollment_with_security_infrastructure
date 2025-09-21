using CourseEnrollment.Models.ManyToMany;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseEnrollment.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("TblEmployees");
        builder.HasKey(k => k.EmployeeId);
        builder.HasMany(r => r.EmployeeRoles)
            .WithOne(er => er.Employee);
    }
}
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("TblRole");
        builder.HasKey(k => k.RoleId);
        builder.HasMany(r => r.EmployeeRoles)
            .WithOne(er => er.Role);
    }
}

public class EmployeeRoleConfiguration : IEntityTypeConfiguration<EmployeeRole>
{
    public void Configure(EntityTypeBuilder<EmployeeRole> builder)
    {
        builder.ToTable("TblEmployeeRole");
        builder.HasKey(k => new {k.EmployeeId,k.RoleId});

        builder.HasOne(er => er.Employee)
            .WithMany(er => er.EmployeeRoles)
            .HasForeignKey(f => f.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(er => er.Role)
            .WithMany(er => er.EmployeeRoles)
            .HasForeignKey(f => f.RoleId)   // make sure RoleId is there in role instance
            .OnDelete(DeleteBehavior.Restrict);
    }
}
