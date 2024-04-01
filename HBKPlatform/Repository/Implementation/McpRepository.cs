using HBKPlatform.Database;
using HBKPlatform.Exceptions;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MissingMemberException = System.MissingMemberException;

namespace HBKPlatform.Repository.Implementation
{
    /// <summary>
    /// HBKPlatform Master Control Panel repository
    /// "Now you're under control"
    /// 
    /// Breaks the paradigm, contains all operations for the MCP irrespective of entity.
    /// This is for security - we can isolate IgnoreQueryFilters and make it obvious this call is needed.
    /// 
    /// Author: Mark Brown
    /// Authored: 23/02/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class McpRepository(ApplicationDbContext _db, IPasswordHasher<User> passwordHasher, UserManager<User> _userMgr, 
        IUserRepository _userRepo, ITenancyService _tenancySrv, ILogger<McpRepository> _logger) : IMcpRepository
    {
    
        /// <summary>
        /// MCP only. Get a Practice.
        /// </summary>
        /// <returns>Practice</returns>
        public async Task<PracticeDetailsDto> GetPracticeAlone(int practiceId)
        {
            return await _db.Practices.IgnoreQueryFilters().Include("LeadPractitioner").Include("Tenancy").Where(x => x.Id == practiceId).Select(x => new PracticeDetailsDto()
            {
                OrgName = x.Tenancy.OrgName,
                OrgTagline = x.Tenancy.OrgTagline,
                Email = x.EmailAddress,
                Id = x.Id,
                LicenceStatus = x.Tenancy.LicenceStatus,
                Telephone = x.Telephone,
                RegistrationDate = x.Tenancy.RegistrationDate,
                StreetAddress = x.StreetAddress,
                LeadPractitionerFullName  = $"{x.LeadPractitioner.Title} {x.LeadPractitioner.Forename} {x.LeadPractitioner.Surname}",
                LeadPractitionerId = x.LeadPractitionerId.HasValue ? x.LeadPractitionerId.Value : -1  // Enforce integrity elsewhere
            }).FirstOrDefaultAsync() ?? throw new IdxNotFoundException($"Could not find practice ID {practiceId}");
        }

        public async Task<List<PracticeDetailsLite>> GetPracticeDetailsLite()
        {
            return await _db.Practices.IgnoreQueryFilters().Include("Tenancy").OrderBy(x => x.Id).Select(x => new PracticeDetailsLite() { Id = x.Id, Name = x.Tenancy.OrgName }).ToListAsync();
        }

        public async Task UpdatePracticeDetails(PracticeDto practice)
        {
            var dbPractice = await _db.Practices.IgnoreQueryFilters().Include("Tenancy").FirstOrDefaultAsync(x => x.Id == practice.Id) ??
                           throw new IdxNotFoundException($"Could not find practice ID {practice.Id}");
            dbPractice.Tenancy.OrgName = practice.OrgName;
            dbPractice.Tenancy.OrgTagline = practice.OrgTagline;
            dbPractice.Telephone = practice.Telephone;
            dbPractice.Tenancy.LicenceStatus = practice.LicenceStatus;
            dbPractice.EmailAddress = practice.Email;
            dbPractice.StreetAddress = practice.StreetAddress;
            await _db.SaveChangesAsync();
        }

        public async Task<Tenancy> RegisterPractice(PracticeRegistrationDto practice)
        {
            if (await _userRepo.IsEmailInUse(practice.LeadPracEmail)) 
                throw new InvalidUserOperationException("Email address already in use");

            var saTenancyId = _tenancySrv.TenancyId;
        
            var tenancy = new Tenancy()
            {
                OrgName = practice.OrgName,
                OrgTagline = practice.OrgTagline,
                RegistrationDate = DateTime.UtcNow,
                LicenceStatus = practice.LicenceStatus,
                ContactEmail = practice.Email
            };
        
            await _db.AddAsync(tenancy);
            await _db.SaveChangesAsync();
        
            // Questionable - but required workaround to ensure new users do not get the 'NowDoctor Admin' as their tenancy.
            _tenancySrv.SetTenancyId(tenancy.Id);

            var user = new User()
            {
                Email = practice.LeadPracEmail,
                NormalizedEmail = practice.LeadPracEmail.ToUpper(),
                UserName = practice.LeadPracEmail,
                NormalizedUserName = practice.LeadPracEmail.ToUpper(),
                EmailConfirmed = true,
                LockoutEnabled = true,
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
                Title = practice.LeadPracTitle,
                Forename = practice.LeadPracForename,
                Surname = practice.LeadPracSurname,
                DateOfBirth = DateOnly.FromDateTime(practice.LeadPracDOB),
                User = user,
                Tenancy = tenancy
            };
        
            var dbPractice = new Practice()
            {
                Description = practice.OrgName,
                EmailAddress = practice.Email,
                Telephone = practice.Telephone,
                StreetAddress = practice.StreetAddress,
                Practitioners = new List<Practitioner>() {prac},
                Tenancy = tenancy
            };
        
            await _db.AddAsync(dbPractice);
            // todo - make resilient?
            await _db.SaveChangesAsync();
            await _userMgr.AddToRoleAsync(user, "Practitioner");
            dbPractice.LeadPractitioner = prac;
            await _db.SaveChangesAsync();
        
            // Now the DB has committed the changes, set the tenancy back to what it was.
            _tenancySrv.SetTenancyId(saTenancyId);

            return tenancy;
        }
    
        public async Task<List<UserDetailsUac>> GetPracticePracs(int practiceId)
        {
            return await _db.Practitioners.IgnoreQueryFilters().Where(x => x.PracticeId == practiceId)
                .Select(x => new UserDetailsUac() { Id = x.Id, Name = $"{x.Title}. {x.Forename} {x.Surname}"}).ToListAsync();
        }

        public async Task<Dictionary<int, UserDetailsUac>> GetPractitionerLockoutStatusDict(int practiceId)
        {
            return await _db.Practitioners.IgnoreQueryFilters().Include("User").Where(x => x.UserId != null && x.User.LockoutEnabled && x.PracticeId == practiceId)
                .ToDictionaryAsync(x => x.Id, x => new UserDetailsUac() { Id = x.Id, Name = $"{x.Title}. {x.Forename} {x.Surname}", HasLockout = x.User.LockoutEnd > DateTime.UtcNow, LockoutEnd = x.User.LockoutEnd });
        }
        
        public async Task<UserDetailsUac> GetLeadManagerLockoutStatus(int clinicId)
        {
            return await _db.Clinics.IgnoreQueryFilters().Include("ManagerUser")
                .Where(x => x.Id == clinicId && x.ManagerUser.LockoutEnabled)
                .Select(x => new UserDetailsUac()
                {
                    Id = x.Id, Name = x.ManagerUser.FullName ?? "", HasLockout = x.ManagerUser.LockoutEnd > DateTime.UtcNow,
                    LockoutEnd = x.ManagerUser.LockoutEnd
                })
                .FirstOrDefaultAsync() ?? throw new IdxNotFoundException($"Could not find lead manager for clinicId {clinicId}");
        }
        
        /// <summary>
        /// MCP ONLY.
        /// Get a list of UserDtos with a LastLogin date greater than 1 week ago.
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserDto>> GetRecentLogins()
        {
            var oneWeekAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(7));
            return await _db.Users.IgnoreQueryFilters().Where(x => x.LastLogin.HasValue && x.LastLogin > oneWeekAgo)
                .OrderBy(x => x.LastLogin)
                .Select(x => new UserDto(){TenancyId = x.TenancyId, UserEmail = x.Email ?? "", LastLogin = x.LastLogin, UserRole = "TODO", LoginCount = x.LoginCount, LockoutEnd = x.LockoutEnd}).ToListAsync();
        }

        public async Task<int> GetRegisteredUserCount()
        {
            return await _db.Users.IgnoreQueryFilters().CountAsync();
        }
        
        public async Task<string> GetPracUserId(int pracId)
        {
            var practitioner = await _db.Practitioners.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == pracId);
            if (practitioner == null || string.IsNullOrWhiteSpace(practitioner.UserId))
            {
                throw new MissingMemberException("Could not find practitioner user");
            }
            
            if (string.IsNullOrWhiteSpace(practitioner.UserId))
                _logger.LogWarning($"UAC attempted on null userId for pracId {pracId}");
            
            return practitioner.UserId;
        }
        
        public async Task<string> GetClientUserId(int clientId)
        {
            var client = await _db.Clients.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == clientId);
            if (client == null || string.IsNullOrWhiteSpace(client.UserId))
            {
                throw new MissingMemberException("UserID is null");
            }

            if (string.IsNullOrWhiteSpace(client.UserId))
                _logger.LogWarning($"UAC attempted on null userId for clientId {clientId}");
            
            return client.UserId;
        }
        
        public async Task<string> GetLeadManagerUserId(int clinicId)
        {
            var userId = (await _db.Clinics.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == clinicId))?.ManagerUserId;

            if (string.IsNullOrWhiteSpace(userId))
                _logger.LogWarning($"UAC attempted on null lead manager user Id for clinicId {clinicId}");
            
            return userId;
        }
        
        //////////////////////////////////////////////////////////////////////////////// 
        // CLINIC METHODS
        //////////////////////////////////////////////////////////////////////////////// 
        public async Task<List<ClinicLite>> GetClinicDetailsLite()
        {
            return await _db.Clinics.IgnoreQueryFilters().Include("Tenancy").OrderBy(x => x.Id).Select(x => new ClinicLite() { Id = x.Id, Name = x.Tenancy.OrgName }).ToListAsync();
        }
        
        public async Task<ClinicDetailsDto> GetClinicAlone(int clinicId)
        {
            return await _db.Clinics.IgnoreQueryFilters().Include("ManagerUser").Include("Tenancy").Where(x => x.Id == clinicId).Select(x => new ClinicDetailsDto()
            {
                OrgName = x.Tenancy.OrgName,
                OrgEmail = x.EmailAddress,
                Id = x.Id,
                LicenceStatus = x.Tenancy.LicenceStatus,
                Telephone = x.Telephone,
                RegistrationDate = x.Tenancy.RegistrationDate,
                StreetAddress = x.StreetAddress ?? "",
                LeadManagerFullName  = x.ManagerUser.FullName ?? "",
                LeadManagerEmail = x.ManagerUser.Email ?? ""
            }).FirstOrDefaultAsync() ?? throw new IdxNotFoundException($"Could not find clinic ID {clinicId}");
        }
        
        public async Task RegisterClinic(ClinicRegistrationDto clinic)
        {
            if (await _userRepo.IsEmailInUse(clinic.LeadManagerEmail)) 
                throw new InvalidUserOperationException("Email address already in use");

            var saTenancyId = _tenancySrv.TenancyId;
        
            var tenancy = new Tenancy()
            {
                OrgName = clinic.OrgName,
                RegistrationDate = DateTime.UtcNow,
                LicenceStatus = clinic.LicenceStatus,
                ContactEmail = clinic.OrgEmail
            };
        
            await _db.AddAsync(tenancy);
            await _db.SaveChangesAsync();
        
            // Questionable - but required workaround to ensure new users do not get the 'NowDoctor Admin' as their tenancy.
            _tenancySrv.SetTenancyId(tenancy.Id);

            var user = new User()
            {
                FullName = $"{clinic.LeadManagerTitle} {clinic.LeadManagerForename} {clinic.LeadManagerSurname}",
                Email = clinic.LeadManagerEmail,
                NormalizedEmail = clinic.LeadManagerEmail.ToUpper(),
                UserName = clinic.LeadManagerEmail,
                NormalizedUserName = clinic.LeadManagerEmail.ToUpper(),
                EmailConfirmed = true,
                LockoutEnabled = true,
                PhoneNumber = "",
                PhoneNumberConfirmed = true,
                Tenancy = tenancy
            };
        
            var pwdGen = new PasswordGenerator.Password(DefaultSettings.DEFAULT_PASSWORD_LENGTH);
            var pwd = pwdGen.Next();
            // TODO: REMOVE THIS!!!
            Console.WriteLine($"DEBUG: PASSWORD IS ====>\n{pwd}\n");
            user.PasswordHash = passwordHasher.HashPassword(user, pwd);
            await _db.AddAsync(user);
            await _db.SaveChangesAsync();
        
            var dbClinic = new Clinic()
            {
                EmailAddress = clinic.OrgEmail,
                Telephone = clinic.Telephone,
                StreetAddress = clinic.StreetAddress,
                Tenancy = tenancy,
                ManagerUser = user
            };
        
            await _db.AddAsync(dbClinic);
            // todo - make resilient?
            await _userMgr.AddToRoleAsync(user, "ClinicManager");
            await _db.SaveChangesAsync();
        
            // Now the DB has committed the changes, set the tenancy back to what it was.
            _tenancySrv.SetTenancyId(saTenancyId);
        }
        
        public async Task UpdateClinicDetails(ClinicDto clinic)
        {
            var dbClinic = await _db.Clinics.IgnoreQueryFilters().Include("Tenancy").FirstOrDefaultAsync(x => x.Id == clinic.Id) ??
                           throw new IdxNotFoundException($"Could not find clinic ID {clinic.Id}");
            dbClinic.Tenancy.OrgName = clinic.OrgName;
            dbClinic.Telephone = clinic.Telephone;
            dbClinic.Tenancy.LicenceStatus = clinic.LicenceStatus;
            dbClinic.EmailAddress = clinic.OrgEmail;
            dbClinic.StreetAddress = clinic.StreetAddress;
            await _db.SaveChangesAsync();
        }
    
    }
}
