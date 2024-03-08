using System.Collections.Concurrent;
using HBKPlatform.Models;

namespace HBKPlatform.Services;

public interface ICentralScrutinizerService
{
    public void PruneActiveUsers();
    public ConcurrentDictionary<string, ActiveUser> GetActive();
    public void RemoveUser(HttpContext context);
    public int GetActiveCount();
    public void RecordAction(HttpContext context);
}