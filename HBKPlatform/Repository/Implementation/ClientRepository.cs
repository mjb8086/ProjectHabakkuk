using System.Data;
using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

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
    public ClientDto GetClientDetails(int clientId)
    {
        return _db.Clients.Include("User").Where(x => x.Id == clientId).Select(x =>
            new ClientDto()
            {
                Id = x.Id,
                Title = x.Title,
                Sex = x.Sex,
                Forename = x.Forename,
                Surname = x.Surname,
                Address = string.IsNullOrEmpty(x.Address) ? "" : x.Address,
                ClinicId = x.ClinicId,
                DateOfBirth = DateOnly.FromDateTime(x.DateOfBirth),
                Email = x.User.Email ?? "",
                Img = x.Img,
                HasUserAccount = !string.IsNullOrWhiteSpace(x.UserId)
            }
        ).FirstOrDefault() ?? throw new MissingPrimaryKeyException($"Could not find client ID {clientId}");
    }

    public async Task<List<ClientDetailsLite>> GetLiteDetails(int clinicId)
    {
        return await _db.Clients.Select(x => new ClientDetailsLite() { Id = x.Id, Name = $"{x.Forename} {x.Surname}" })
            .ToListAsync();
    }
}