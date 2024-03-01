using HBKPlatform.Models;

namespace HBKPlatform.Services;

public interface ICentralScrutinizerService
{
    public void PruneActiveUsers();
    public Dictionary<string, ActiveUser> GetActive();
    public void RecordAction(HttpContext context);
}