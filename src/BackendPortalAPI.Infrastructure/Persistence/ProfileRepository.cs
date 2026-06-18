using BackendPortalAPI.Application.Profiles;
using BackendPortalAPI.Domain.Entities;
using BackendPortalAPI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackendPortalAPI.Infrastructure.Persistence;

public class ProfileRepository(AppDbContext db) : IProfileRepository
{
    public async Task<(IReadOnlyList<Profile> Items, int Total)> QueryAsync(ProfileQuery q, CancellationToken ct = default)
    {
        var query = db.Profiles.AsNoTracking().AsQueryable();

        if (q.Gender is not null) query = query.Where(p => p.Gender == q.Gender);
        if (!string.IsNullOrWhiteSpace(q.Denomination)) query = query.Where(p => p.Denomination == q.Denomination);
        if (!string.IsNullOrWhiteSpace(q.Congregation)) query = query.Where(p => p.Congregation == q.Congregation);
        if (q.Status is not null) query = query.Where(p => p.Status == q.Status);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<Profile?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Profiles.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Profile?> GetByReferenceIdAsync(string referenceId, CancellationToken ct = default) =>
        db.Profiles.AsNoTracking().FirstOrDefaultAsync(p => p.ReferenceId == referenceId, ct);

    public Task<int> CountAsync(CancellationToken ct = default) => db.Profiles.CountAsync(ct);

    public async Task<bool> UpdateStatusAsync(Guid id, ProfileStatus status, CancellationToken ct = default)
    {
        var profile = await db.Profiles.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (profile is null) return false;
        profile.Status = status;
        profile.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Dictionary<ProfileStatus, int>> CountByStatusAsync(CancellationToken ct = default) =>
        await db.Profiles
            .GroupBy(p => p.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);

    public async Task AddAsync(Profile profile, CancellationToken ct = default) =>
        await db.Profiles.AddAsync(profile, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
