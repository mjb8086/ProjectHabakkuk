using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// Clinic Repository.
///
/// Author: Mark Brown
/// Authored: 13/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
public class ClinicRepository(ApplicationDbContext _db, IPasswordHasher<User> passwordHasher, UserManager<User> _userMgr) : IClinicRepository
{
    /// <summary>
    /// Get a Clinic.
    /// </summary>
    /// <returns>Clinic</returns>
    public async Task<ClinicDetailsDto> GetClinicAlone(int clinicIdx)
    {
        return await _db.Clinics.Include("LeadPractitioner").Where(x => x.Id == clinicIdx).Select(x => new ClinicDetailsDto()
        {
            OrgName = x.OrgName,
            OrgTagline = x.OrgTagline,
            Email = x.EmailAddress,
            Id = x.Id,
            LicenceStatus = x.LicenceStatus,
            Telephone = x.Telephone,
            RegistrationDate = x.RegistrationDate,
            StreetAddress = x.StreetAddress,
            LeadPracFullName  = $"{x.LeadPractitioner.Title} {x.LeadPractitioner.Forename} {x.LeadPractitioner.Surname}",
            LeadPractitionerId = x.LeadPractitionerId.HasValue ? x.LeadPractitionerId.Value : -1  // Enforce integrity elsewhere
        }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Could not find clinic ID {clinicIdx}");
    }

    public async Task<List<ClinicDetailsLite>> GetClinicDetailsLite()
    {
        return await _db.Clinics.OrderBy(x => x.Id).Select(x => new ClinicDetailsLite() { Id = x.Id, Name = x.OrgName }).ToListAsync();
    }

    public async Task UpdateClinicDetails(ClinicDto clinic)
    {
        var dbClinic = await _db.Clinics.FirstOrDefaultAsync(x => x.Id == clinic.Id) ?? throw new KeyNotFoundException($"Could not find clinic ID {clinic.Id}");
        dbClinic.OrgName = clinic.OrgName;
        dbClinic.OrgTagline = clinic.OrgTagline;
        dbClinic.Telephone = clinic.Telephone;
        dbClinic.LicenceStatus = clinic.LicenceStatus;
        dbClinic.EmailAddress = clinic.Email;
        dbClinic.StreetAddress = clinic.StreetAddress;
        await _db.SaveChangesAsync();
    }

    public async Task RegisterClinic(ClinicRegistrationDto clinic)
    {
        if (await IsEmailInUse(clinic.LeadPracEmail)) 
            throw new InvalidOperationException("Email address already in use");
        
        var user = new User()
        {
            Email = clinic.LeadPracEmail,
            NormalizedEmail = clinic.LeadPracEmail.ToUpper(),
            UserName = clinic.LeadPracEmail,
            NormalizedUserName = clinic.LeadPracEmail.ToUpper(),
            EmailConfirmed = true,
            LockoutEnabled = false,
            PhoneNumber = "",
            PhoneNumberConfirmed = true,
        };
        
        var pwdGen = new PasswordGenerator.Password(DefaultSettings.DEFAULT_PASSWORD_LENGTH);
        var pwd = pwdGen.Next();
        // TODO: REMOVE THIS!!!
        Console.WriteLine($"DEBUG: PASSWORD IS ====>\n{pwd}\n");
        user.PasswordHash = passwordHasher.HashPassword(user, pwd);
        
        var prac = new Practitioner()
        {
            Title = clinic.LeadPracTitle,
            Forename = clinic.LeadPracForename,
            Surname = clinic.LeadPracSurname,
            DateOfBirth = clinic.LeadPracDOB,
            User = user
        };
        
        var dbClinic = new Clinic()
        {
            OrgName = clinic.OrgName,
            OrgTagline = clinic.OrgTagline,
            EmailAddress = clinic.Email,
            RegistrationDate = DateTime.UtcNow,
            LicenceStatus = clinic.LicenceStatus,
            Telephone = clinic.Telephone,
            Practitioners = new List<Practitioner>() {prac}
        };
        await _db.AddAsync(dbClinic);
        // todo - make resilient?
        await _db.SaveChangesAsync();
        await _userMgr.AddToRoleAsync(user, "Practitioner");
        dbClinic.LeadPractitioner = prac;
        await _db.SaveChangesAsync();
    }
    
    public async Task<bool> IsEmailInUse(string email)
    {
        return await _db.Users.AnyAsync(x =>
            x.Email != null && x.Email.ToLower() == email || x.UserName != null && x.UserName.ToLower() == email);
    }
    
}