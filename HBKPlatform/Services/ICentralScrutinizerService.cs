using System.Collections.Concurrent;
using HBKPlatform.Models;

namespace HBKPlatform.Services;

public interface ICentralScrutinizerService
{
    public void PruneActiveUsers();
    public ConcurrentDictionary<string, ActiveUser> GetActive();
    public int GetActiveCount();
    public void RecordAction(HttpContext context);
}