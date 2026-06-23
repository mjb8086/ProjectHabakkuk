using System.Collections.Concurrent;
using Hbk.Models;

namespace Hbk.Platform.Services;

public interface ICentralScrutinizerService
{
    public void PruneActiveUsers();
    public ConcurrentDictionary<string, ActiveUser> GetActive();
    public void RemoveUser(HttpContext context);
    public int GetActiveCount();
    public void RecordAction(HttpContext context);
}