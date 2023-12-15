using HBKPlatform.Database;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// Client Repository.
/// 
/// Author: Mark Brown
/// Authored: 15/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
public class ClientRepository(ApplicationDbContext _db) :IClientRepository
{
    public ClientDetailsLite GetLiteDetails(int clientId)
    {
        return _db.Clients.Where(x => x.Id == clientId).Select(x => 
            new ClientDetailsLite() { Name = $"{x.Forename} {x.Surname}" }
        ).First();
    }
}