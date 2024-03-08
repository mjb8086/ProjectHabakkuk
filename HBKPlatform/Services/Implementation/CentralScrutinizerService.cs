using System.Collections.Concurrent;
using System.Security.Claims;
using HBKPlatform.Models;

namespace HBKPlatform.Services.Implementation;

/*
   This is the Central Scrutinizer...
   It is my responsibility to enforce all the laws
   That haven't been passed yet
   It is also my responsibility to alert each
   And every one of you to the potential consequences
   Of various ordinary everyday activities
   You might be performing which could eventually lead to
   The Death Penalty (or affect your parents' credit rating) 
   - F. Zappa
 */

/// <summary>
/// HBKPlatform Central Scrutinizer service
/// Will keep a log of recent users for security purposes.
/// 
/// Author: Mark Brown
/// Authored: 01/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class CentralScrutinizerService : ICentralScrutinizerService
{
    private ConcurrentDictionary<string, ActiveUser> _activeUsers = new();
    private TimeSpan LAST_ACTION_SPAN = TimeSpan.FromHours(1);

    public void PruneActiveUsers()
    {
        foreach (var s in _activeUsers.Where(kv => kv.Value.LastActionTime < DateTime.UtcNow.Subtract(LAST_ACTION_SPAN)).ToList() )
        {
            _activeUsers.TryRemove(s);
        }
    }

    public ConcurrentDictionary<string, ActiveUser> GetActive()
    {
        return _activeUsers;
    }

    public int GetActiveCount()
    {
        return _activeUsers.Count;
    }

    public void RemoveUser(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null) _activeUsers.TryRemove(userId, out _);
    }

    public void RecordAction(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = context.User.FindFirstValue(ClaimTypes.Email);
        var userRole = context.User.FindFirstValue(ClaimTypes.Role);
        var tenancyId = -1;
        var c = context.User.FindFirst("TenancyId");
        if (c != null && int.TryParse(c.Value, out tenancyId)) ;
        var route = context.Request.Path.Value;

        if (userId != null)
        {
            _activeUsers[userId] = new ActiveUser()
            {
                UserEmail = userEmail ?? "NONE", UserRole = userRole ?? "NONE", LastActionTime = DateTime.UtcNow, 
                TenancyId = tenancyId, Path = route ?? ""
            };
        }
    }
    
}