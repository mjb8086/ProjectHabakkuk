using System.Data;
using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    /// <summary>
    /// Client Repository.
    /// 
    /// Author: Mark Brown
    /// Authored: 15/12/2023
    /// 
    /// © 2023 NowDoctor Ltd.
    /// </summary>
    public class ClientRepository(ApplicationDbContext _db, IPasswordHasher<User> passwordHasher, 
        UserManager<User> _userMgr, IUserRepository _userRepo, ITenancyService _tenancySrv) :IClientRepository
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

        public async Task<List<ClientDetailsLite>> GetLiteDetails()
        {
            return await _db.Clients
                .Select(x => new ClientDetailsLite() { Id = x.Id, Name = $"{x.Forename} {x.Surname}" })
                .ToListAsync();
        }

        public async Task Create(ClientDto client)
        {
            bool willHaveUser = client.HasUserAccount && !string.IsNullOrWhiteSpace(client.Email);
            var dbClient = new Database.Client()
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
            };
        
            if (willHaveUser)
            {
                if (await _userRepo.IsEmailInUse(client.Email)) throw new InvalidOperationException("Email address already in use");
                var user = new User()
                {
                    Email = client.Email,
                    NormalizedEmail = client.Email.ToUpper(), // previous check ensures this is not null
                    UserName = client.Email,
                    NormalizedUserName = client.Email.ToUpper(),
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    PhoneNumber = client.Telephone,
                    PhoneNumberConfirmed = true,
                    TenancyId = _tenancySrv.TenancyId
                };
                var pwdGen = new PasswordGenerator.Password(DefaultSettings.DEFAULT_PASSWORD_LENGTH);
                var pwd = pwdGen.Next();
                // TODO: REMOVE THIS!!! In place until we have an email client
                Console.WriteLine($"DEBUG: PASSWORD IS ====>\n{pwd}\n");
                user.PasswordHash = passwordHasher.HashPassword(user, pwd);
                dbClient.User = user;
            }
        
            await _db.AddAsync(dbClient);
            var clientPrac = new ClientPractitioner() { Client = dbClient, PractitionerId = client.PractitionerId };
            await _db.AddAsync(clientPrac);
        
            await _db.SaveChangesAsync();
            if (willHaveUser) await _userMgr.AddToRoleAsync(dbClient.User, "Client");
        }

        public async Task Update(ClientDto client)
        {
            var dbClient = await _db.Clients.Include("User").FirstOrDefaultAsync(x => client.Id == x.Id) 
                           ?? throw new KeyNotFoundException($"ClientID {client.Id} not found.");
            dbClient.Title = client.Title;
            dbClient.Forename = client.Forename;
            dbClient.Surname = client.Surname;
            dbClient.Address = client.Address;
            dbClient.Telephone = client.Telephone;
            dbClient.Sex = client.Sex;
            dbClient.DateOfBirth = client.DateOfBirth;
            dbClient.Img = client.Img;

            if (!(string.IsNullOrEmpty(dbClient.UserId) || string.IsNullOrEmpty(client.Email)))
            {
                if (await _userRepo.IsEmailInUse(client.Email, dbClient.User.Email)) throw new InvalidOperationException("Email address already in use");
                dbClient.User.Email = client.Email;
            }
        
            await _db.SaveChangesAsync();
        }

        public int GetClientCount()
        {
            return _db.Clients.Count();
        }

    }
}