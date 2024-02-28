using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    /// <summary>
    /// HBKPlatform MCP repository
    /// "Now you're under control"
    /// 
    /// Breaks the paradigm but contains all operations for the MCP irrespective of entity.
    /// This is for security - we can isolate IgnoreQueryFilters and make it obvious this call is needed.
    /// 
    /// Author: Mark Brown
    /// Authored: 23/02/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class McpRepository(ApplicationDbContext _db, IPasswordHasher<User> passwordHasher, UserManager<User> _userMgr, 
        IUserRepository _userRepo, ITenancyService _tenancySrv) : IMcpRepository
    {
    
        /// <summary>
        /// MCP only. Get a Clinic.
        /// </summary>
        /// <returns>Clinic</returns>
        public async Task<ClinicDetailsDto> GetClinicAlone(int clinicIdx)
        {
            return await _db.Clinics.IgnoreQueryFilters().Include("LeadPractitioner").Include("Tenancy").Where(x => x.Id == clinicIdx).Select(x => new ClinicDetailsDto()
            {
                OrgName = x.Tenancy.OrgName,
                OrgTagline = x.Tenancy.OrgTagline,
                Email = x.EmailAddress,
                Id = x.Id,
                LicenceStatus = x.Tenancy.LicenceStatus,
                Telephone = x.Telephone,
                RegistrationDate = x.Tenancy.RegistrationDate,
                StreetAddress = x.StreetAddress,
                LeadPracFullName  = $"{x.LeadPractitioner.Title} {x.LeadPractitioner.Forename} {x.LeadPractitioner.Surname}",
                LeadPractitionerId = x.LeadPractitionerId.HasValue ? x.LeadPractitionerId.Value : -1  // Enforce integrity elsewhere
            }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Could not find clinic ID {clinicIdx}");
        }

        public async Task<List<ClinicDetailsLite>> GetClinicDetailsLite()
        {
            return await _db.Clinics.IgnoreQueryFilters().Include("Tenancy").OrderBy(x => x.Id).Select(x => new ClinicDetailsLite() { Id = x.Id, Name = x.Tenancy.OrgName }).ToListAsync();
        }

        public async Task UpdateClinicDetails(ClinicDto clinic)
        {
            var dbClinic = await _db.Clinics.IgnoreQueryFilters().Include("Tenancy").FirstOrDefaultAsync(x => x.Id == clinic.Id) ??
                           throw new KeyNotFoundException($"Could not find clinic ID {clinic.Id}");
            dbClinic.Tenancy.OrgName = clinic.OrgName;
            dbClinic.Tenancy.OrgTagline = clinic.OrgTagline;
            dbClinic.Telephone = clinic.Telephone;
            dbClinic.Tenancy.LicenceStatus = clinic.LicenceStatus;
            dbClinic.EmailAddress = clinic.Email;
            dbClinic.StreetAddress = clinic.StreetAddress;
            await _db.SaveChangesAsync();
        }

        public async Task<Tenancy> RegisterClinic(ClinicRegistrationDto clinic)
        {
            if (await _userRepo.IsEmailInUse(clinic.LeadPracEmail)) 
                throw new InvalidOperationException("Email address already in use");

            var saTenancyId = _tenancySrv.TenancyId;
        
            var tenancy = new Tenancy()
            {
                OrgName = clinic.OrgName,
                OrgTagline = clinic.OrgTagline,
                RegistrationDate = DateTime.UtcNow,
                LicenceStatus = clinic.LicenceStatus,
                ContactEmail = clinic.Email
            };
        
            await _db.AddAsync(tenancy);
            await _db.SaveChangesAsync();
        
            // Very questionable - but required workaround to ensure new users do not get the 'NowDoctor Admin' as their tenancy.
            _tenancySrv.SetTenancyId(tenancy.Id);

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
                Tenancy = tenancy
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
                DateOfBirth = DateOnly.FromDateTime(clinic.LeadPracDOB),
                User = user,
                Tenancy = tenancy
            };
        
            var dbClinic = new Clinic()
            {
                Description = clinic.OrgName,
                EmailAddress = clinic.Email,
                Telephone = clinic.Telephone,
                StreetAddress = clinic.StreetAddress,
                Practitioners = new List<Practitioner>() {prac},
                Tenancy = tenancy
            };
        
            await _db.AddAsync(dbClinic);
            // todo - make resilient?
            await _db.SaveChangesAsync();
            await _userMgr.AddToRoleAsync(user, "Practitioner");
            dbClinic.LeadPractitioner = prac;
            await _db.SaveChangesAsync();
        
            // Now the DB has committed the changes, set the tenancy back to what it was.
            _tenancySrv.SetTenancyId(saTenancyId);

            return tenancy;
        }
    
        public async Task<List<PracDetailsUac>> GetClinicPracs(int clinicId)
        {
            return await _db.Practitioners.IgnoreQueryFilters().Where(x => x.ClinicId == clinicId)
                .Select(x => new PracDetailsUac() { Id = x.Id, Name = $"{x.Title}. {x.Forename} {x.Surname}"}).ToListAsync();
        }

        public async Task<Dictionary<int, bool>> GetPracLockoutStatusDict(int clinicId)
        {
            return await _db.Practitioners.IgnoreQueryFilters().Include("User").Where(x => x.UserId != null)
                .ToDictionaryAsync(x => x.Id, x => x.User.LockoutEnabled);
        }
    
    }
}