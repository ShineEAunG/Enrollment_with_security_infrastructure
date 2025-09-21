using CourseEnrollment.Data;
using CourseEnrollment.Models.Authentications;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Repository;

public class RefreshTokenRepo
{
    private readonly AppDbContext _context;
    public RefreshTokenRepo(AppDbContext context)
    {
        this._context = context;
    }

    public async Task<RefreshToken?> GetByHash(string hashToken)
    {
        var token = await _context.RefreshTokens.Include(i => i.Employee).FirstOrDefaultAsync(t => t.TokenHash == hashToken);
        return token;
    }

    public async Task AddRefreshToken(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }
    public async Task<bool> SaveRefreshTokenInDb()
    {
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task RevokeRefreshToken(string employeeId) // global log out if we want to specific session to log out we need tokenId
    {
        var tokens = await _context.RefreshTokens.Where(u => u.EmployeeId == employeeId && u.RevokeAt == null && u.IsActive == true).ToListAsync();
        foreach (var token in tokens)
        {
            token.RevokeAt = DateTime.UtcNow;
        }
    }
}