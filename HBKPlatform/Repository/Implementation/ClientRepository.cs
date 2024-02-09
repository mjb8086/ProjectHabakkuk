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
                DateOfBirth = x.DateOfBirth,
                Email = x.User.Email ?? "",
                Img = x.Img,
                Telephone = x.Telephone,
                HasUserAccount = !string.IsNullOrWhiteSpace(x.UserId)
            }
        ).FirstOrDefault() ?? throw new MissingPrimaryKeyException($"Could not find client ID {clientId}");
    }

    public async Task<List<ClientDetailsLite>> GetLiteDetails(int clinicId)
    {
        return await _db.Clients.Where(x => x.ClinicId == clinicId)
            .Select(x => new ClientDetailsLite() { Id = x.Id, Name = $"{x.Forename} {x.Surname}" })
            .ToListAsync();
    }

    public async Task Create(ClientDto client)
    {
        await _db.AddAsync(new Database.Client()
        {
            Title = client.Title,
            Forename = client.Forename,
            Surname = client.Surname,
            Address = client.Address,
            ClinicId = client.ClinicId,
            Telephone = client.Telephone,
            Sex = client.Sex,
            DateOfBirth = client.DateOfBirth,
            Img = client.Img
        });
        // Todo - register new user and send email, will require user service...
        await _db.SaveChangesAsync();
    }

    public async Task Update(ClientDto client)
    {
        var dbClient = await _db.Clients.FirstOrDefaultAsync(x => client.Id == x.Id) 
                       ?? throw new KeyNotFoundException($"ClientID {client.Id} not found.");
        dbClient.Title = client.Title;
        dbClient.Forename = client.Forename;
        dbClient.Surname = client.Surname;
        dbClient.Address = client.Address;
        dbClient.Telephone = client.Telephone;
        dbClient.Sex = client.Sex;
        dbClient.DateOfBirth = client.DateOfBirth;
        dbClient.Img = client.Img;
        
        await _db.SaveChangesAsync();
            // todo - user service update email
    }

    public int GetClientCount(int clinicId)
    {
        return _db.Clients.Count(x => x.ClinicId == clinicId);
    }
}