using BackendPortalAPI.Application.Members;
using BackendPortalAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendPortalAPI.Infrastructure.Persistence;

public class MemberRepository(AppDbContext db) : IMemberRepository
{
    public Task<Member?> FindByCardAsync(string membershipNo, CancellationToken ct = default) =>
        db.Members.AsNoTracking().FirstOrDefaultAsync(m => m.MembershipNo == membershipNo, ct);

    public Task<Member?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);
}
