using HBKPlatform.Database;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Services.Implementation;

public class ClinicService(ApplicationDbContext _db) : IClinicService
{
    public async Task<bool> VerifyClientAndPracClinicMembership(int clientId, int pracId)
    {
        var client = await _db.Clients.FirstAsync(x => x.Id == clientId);
        var prac = await _db.Practitioners.FirstAsync(x => x.Id == pracId);
        return client.ClinicId == prac.ClinicId;
    }
}