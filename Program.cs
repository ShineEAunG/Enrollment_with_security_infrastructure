using CourseEnrollment.Data;
using CourseEnrollment.Data.Seeding;
using CourseEnrollment.Extensions;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.DbContextConfig(builder.Configuration);
builder.Services.JwtSchemeConfiguration(builder.Configuration);
builder.Services.AddControllerConfig();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        if (!context.Students.Any())
        {
            StudentSeedData.SeedData(context);
        }
        if (!context.Roles.Any())
        {
            RoleSeedData.SeedRoles(context);
        }
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
