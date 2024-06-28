using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Services.Implementation;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database.Helpers
{
    // Sneed's
    public static class SeedNFeed //Formerly Chuck's
    {
        public static async Task Initialise(IServiceProvider provider, IPasswordHasher<User> passwordHasher)
        {
            var tenancySrv = new TenancyService();
            List<Timeslot> timeslots;
            
            using (var ctx = new ApplicationDbContext(
               provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>(), new HttpContextAccessor(), tenancySrv))
            {
                var roleStore = new RoleStore<IdentityRole>(ctx);
                IdentityRole? superAdminRole, pracRole, clientRole, clinicMgrRole;

                if ((superAdminRole = ctx.Roles.FirstOrDefault(r => r.Name == "SuperAdmin")) == null)
                {
                    superAdminRole = new IdentityRole() { Name = "SuperAdmin", NormalizedName = "SuperAdmin".ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString()};
                    await roleStore.CreateAsync(superAdminRole);
                }
                
                if ((pracRole = ctx.Roles.FirstOrDefault(r => r.Name == "Practitioner")) == null)
                {
                    pracRole = new IdentityRole() { Name = "Practitioner", NormalizedName = "Practitioner".ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() };
                    await roleStore.CreateAsync(pracRole);
                }
                if ((clientRole = ctx.Roles.FirstOrDefault(r => r.Name == "Client")) == null)
                {
                    clientRole = new IdentityRole() { Name = "Client", NormalizedName = "Client".ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() };
                    await roleStore.CreateAsync(clientRole);
                }
                if ((clinicMgrRole = ctx.Roles.FirstOrDefault(r => r.Name == "ClinicManager")) == null)
                {
                    clinicMgrRole = new IdentityRole() { Name = "ClinicManager", NormalizedName = "ClinicManager".ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() };
                    await roleStore.CreateAsync(clinicMgrRole);
                }


                if (ctx.Timeslots.Any())
                {
                    timeslots = ctx.Timeslots.ToList();
                }
                else
                {
                    timeslots = TimeslotHelper.GenerateDefaultTimeslots(TimeslotHelper.DEFAULT_START,
                        TimeslotHelper.DEFAULT_END);
                    await ctx.AddRangeAsync(timeslots);
                    await ctx.SaveChangesAsync();
                }

                if (!ctx.Tenancies.Any() && !ctx.Practitioners.Any() && !ctx.Clients.Any() && !ctx.Practices.Any()) {

                    var ndTenancy = new Tenancy()
                    {
                        OrgName = "NowDoctor Admin",
                        ContactEmail = ".",
                        LicenceStatus = Enums.LicenceStatus.Active,
                        RegistrationDate = DateTime.UtcNow,
                        OrgTagline = "",
                        Type = TenancyType.NdAdmin
                    };
                    
                    var t = new Tenancy()
                    {
                        OrgName = "Lawrence Street Practice",
                        ContactEmail = "foo@bar.net",
                        LicenceStatus = Enums.LicenceStatus.Active,
                        RegistrationDate = DateTime.UtcNow,
                        OrgTagline = "Timely treatment or your time back.",
                        Type = TenancyType.Practice
                    };
                    
                    var suEmail = "mjb+sudo1@nowdoctor.co.uk";
                    var superUser = new User()
                    {
                        Email = suEmail,
                        NormalizedEmail = suEmail.ToUpper(),
                        UserName = suEmail,
                        NormalizedUserName = suEmail.ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "0898 333 201",
                        PhoneNumberConfirmed = true,
                        Tenancy = ndTenancy
                    };
                    superUser.PasswordHash = passwordHasher.HashPassword(superUser, "changeme123");
                    
                    await ctx.AddAsync(superUser);
                    await ctx.AddAsync(ndTenancy);
                    await ctx.AddAsync(t);
                    await ctx.SaveChangesAsync();
                    
                    // Set tenancyId to demo tenancy to avoid FK violations on CREATE actions (see AppDbCtx).
                    // CREATE PRACS on T1
                    tenancySrv.SetTenancyId(t.Id);
                    
                    var user1Email = "drwallace@lawrencestreetpractice.com";
                    var user1 = new User()
                    {
                        Email = user1Email,
                        NormalizedEmail = user1Email.ToUpper(),
                        UserName = user1Email.ToUpper(),
                        NormalizedUserName = user1Email.ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = true,
                        PhoneNumber = "0898 333 201",
                        PhoneNumberConfirmed = true,
                        Tenancy = t
                    };
                    user1.PasswordHash = passwordHasher.HashPassword(user1, "trustmeiamadoctor");
                    
                    var prac1 = new Practitioner()
                    {
                        Forename = "Roger",
                        Surname = "Wallace",
                        Title = Enums.Title.Dr,
                        ClientBio = "10 year's experience in a field",
                        Location = "Belfast",
                        DateOfBirth = new DateOnly(1992, 07, 08),
                        Img = new string("/samples/wallace.jpg"),
                        User = user1,
                        GmcNumber = "1234",
                        Tenancy = t
                    };
                    
                    var prac2 = new Practitioner()
                    {
                        Forename = "Another",
                        Surname = "Prac",
                        Title = Enums.Title.Dr,
                        ClientBio = "layabout",
                        Location = "the pub",
                        DateOfBirth = new DateOnly(1992, 07, 08),
                        Img = new string("/samples/second.jpg"),
                        Tenancy = t,
                        GmcNumber = "foo",
                        User =  new ()
                        {
                            Email = "another@hillvalley.com",
                            NormalizedEmail = "another@hillvalley.com".ToUpper(),
                            UserName = "another@hillvalley.com",
                            NormalizedUserName = "another@hillvalley.com".ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = true,
                            PhoneNumber = "0898 333 201",
                            PhoneNumberConfirmed = true,
                            Tenancy = t
                        }
                    };
                    prac2.User.PasswordHash = passwordHasher.HashPassword(prac2.User, "trustmeiamadoctor");

                    var client1Email = "edward@fsmail.net";
                    var client1User = new User()
                    {
                        Email = client1Email,
                        NormalizedEmail = client1Email.ToUpper(),
                        UserName = client1Email,
                        NormalizedUserName = client1Email.ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = true,
                        PhoneNumber = "0898 333 201",
                        PhoneNumberConfirmed = true,
                        Tenancy = t
                    };
                    client1User.PasswordHash = passwordHasher.HashPassword(client1User, "eddie_metal");

                    var setting = new Setting()
                    {
                        Key = "SelfBookingEnabled",
                        Value = "True"
                    };
                    ctx.Add(setting);
                    
                    var client1 = new Client()
                    {
                        Forename = "Edward",
                        Surname = "Stewart",
                        Title = Enums.Title.Mr,
                        Address = "a garage",
                        DateOfBirth = new DateOnly(1992, 07, 08),
                        Img = new string("/samples/edward.jpg"),
                        Telephone = "999",
                        User = client1User,
                        Tenancy = t
                    };
                    
                    var client2Email = "laura@hotmail.com";
                    var client2User = new User()
                    {
                        Email = client2Email,
                        NormalizedEmail = client2Email.ToUpper(),
                        UserName = client2Email,
                        NormalizedUserName = client2Email.ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = true,
                        PhoneNumber = "0898 333 201",
                        PhoneNumberConfirmed = true,
                        Tenancy = t
                    };
                    client2User.PasswordHash = passwordHasher.HashPassword(client1User, "ihatemanure");
                    
                    var client2 = new Client()
                    {
                        Forename = "Laura",
                        Surname = "McMaster",
                        Title = Enums.Title.Ms,
                        Address = "A big house somewhere",
                        DateOfBirth = new DateOnly(1972, 07, 08),
                        Img = "/samples/laura.jpg",
                        Telephone = "299",
                        User = client2User,
                        Tenancy = t
                    };
                    
                    var practice = new Practice()
                    {
                        EmailAddress = "foo@bar.com",
                        Description = "Lawrence Street Practice",
                        Telephone = "0898 333 201",
                        Clients = new List<Client>() {client1, client2},
                        Practitioners = new List<Practitioner>() {prac1, prac2},
                        Tenancy = t
                    };

                    ctx.Add(practice);
                    ctx.SaveChanges();

                    practice.LeadPractitioner = prac1;
                    
                    var clientPracs = new List<ClientPractitioner>()
                    {
                        new () {Client = client1, Practitioner = prac1, Tenancy = t},
                        new () {Client = client2, Practitioner = prac1, Tenancy = t},
                    };

                    ctx.AddRange(clientPracs);
                    ctx.SaveChanges();

                    
                    var roles = new List<IdentityUserRole<string>>
                    {
                        new () { UserId = user1.Id, RoleId = pracRole.Id },
                        new () { UserId = prac2.User.Id, RoleId = pracRole.Id },
                        new () { UserId = client1User.Id, RoleId = clientRole.Id },
                        new () { UserId = client2User.Id, RoleId = clientRole.Id },
                        new () { UserId = superUser.Id, RoleId = superAdminRole.Id }
                    };
                    
                    ctx.AddRange(roles);

                    var conversation = new List<ClientMessage>();
                    conversation.Add(new ()
                    {
                        ClientId = client1.Id, PractitionerId = prac1.Id, MessageOrigin = Enums.MessageOrigin.Client,
                        MessageBody = "Help my head is inflamed", Tenancy = t
                    });
                    conversation.Add(new ()
                    {
                        ClientId = client1.Id, PractitionerId = prac1.Id, MessageOrigin = Enums.MessageOrigin.Practitioner,
                        MessageBody = "That sounds painful, when did you first notice?", Tenancy = t
                    });
                    conversation.Add(new ()
                    {
                        ClientId = client2.Id, PractitionerId = prac1.Id,  MessageOrigin = Enums.MessageOrigin.Practitioner,
                        MessageBody = "Morning!", Tenancy = t
                    });
                    ctx.AddRange(conversation);

                    var clientRecord1 = new ClientRecord()
                    {
                        Client = client1, RecordVisibility = Enums.RecordVisibility.ClientAndPrac,
                        Title = "", NoteBody = "bother shifting the goods", Practitioner = prac1, Tenancy = t
                    };
                    ctx.Add(clientRecord1);

                    var treatment1 = new Treatment()
                    {
                        TreatmentRequestability = Enums.TreatmentRequestability.ClientAndPrac,
                        Title = "Checkup",
                        Description = "talk and pretend to do something",
                        Cost = 15.50, 
                        Tenancy = t
                    };
                    
                    var treatment2 = new Treatment()
                    {
                        TreatmentRequestability = Enums.TreatmentRequestability.PracOnly,
                        Title = "Prac Only Test",
                        Description = "no book for client",
                        Cost = 69.0,
                        Tenancy = t
                    };

                    ctx.Add(treatment1);
                    ctx.Add(treatment2);

                    ctx.SaveChanges();

                    var appointments = new Appointment[] { 
                        new ()
                        {
                            Client = client1,
                            Practitioner = prac1,
                            Timeslot = timeslots[1],
                            Status = Enums.AppointmentStatus.Live,
                            Treatment = treatment1,
                            WeekNum = 4,
                            Tenancy = t
                        },
                        new ()
                        {
                            Client = client1,
                            Practitioner = prac1,
                            Timeslot = timeslots[2],
                            Status = Enums.AppointmentStatus.CancelledByClient,
                            CancellationReason = "Someone else does it cheaper, sorry",
                            Treatment = treatment1,
                            WeekNum = 30,
                            Tenancy = t
                        },
                        new ()
                        {
                            Client = client1,
                            Practitioner = prac1,
                            Timeslot = timeslots[20],
                            Status = Enums.AppointmentStatus.CancelledByPractitioner,
                            CancellationReason = "bollocks",
                            Treatment = treatment2,
                            WeekNum = 30,
                            Tenancy = t
                        }
                    };
                    
                    ctx.AddRange(appointments);

                    var startDate = new Setting()
                    {
                        Key = "DbStartDate",
                        Value = "2024-01-01",
                        Tenancy = t
                    };
                    ctx.Add(startDate);

                    var ta = new List<TimeslotAvailability>()
                    {
                        new()
                        {
                            Timeslot = timeslots[100],
                            Practitioner = prac1,
                            Availability = Enums.TimeslotAvailability.Unavailable,
                            WeekNum = 20,
                            Tenancy = t,
                        },
                        new ()
                        {
                            Timeslot = timeslots[101],
                            Practitioner = prac1,
                            Availability = Enums.TimeslotAvailability.Unavailable,
                            WeekNum = 20,
                            Tenancy = t
                        },
                        new ()
                        {
                            Timeslot = timeslots[102],
                            Practitioner = prac1,
                            Availability = Enums.TimeslotAvailability.Unavailable,
                            WeekNum = 20,
                            Tenancy = t
                        },
                        new ()
                        {
                            Timeslot = timeslots[104],
                            Practitioner = prac1,
                            Availability = Enums.TimeslotAvailability.Available,
                            WeekNum = 20,
                            Tenancy = t
                        },
                    };
                    ctx.AddRange(ta);
                    ctx.SaveChanges();
                    
                    // Now add Clinic and rooms for rental
                    var mgr1Email = "wolseley@btinternet.com";
                    var clinicTenancy = new Tenancy() {
                        OrgName = "Wolseley Street Clinic",
                        LicenceStatus = Enums.LicenceStatus.Active,
                        Type = TenancyType.Clinic,
                        ContactEmail = mgr1Email
                    };
                    await ctx.AddAsync(clinicTenancy);
                    await ctx.SaveChangesAsync();
                    tenancySrv.SetTenancyId(clinicTenancy.Id);
                    
                    var managerUser = new User()
                    {
                        Email = mgr1Email,
                        NormalizedEmail = mgr1Email.ToUpper(),
                        UserName = mgr1Email,
                        NormalizedUserName = mgr1Email.ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = true,
                        PhoneNumber = "98989",
                        PhoneNumberConfirmed = true,
                        Tenancy = clinicTenancy,
                        FullName = "Lucy McGrogan"
                    };
                    managerUser.PasswordHash = passwordHasher.HashPassword(client1User, "vip_pass_mode");
                    await ctx.AddAsync(managerUser);
                    await ctx.SaveChangesAsync();

                    var mgrUser = new IdentityUserRole<string>()
                    {
                        UserId = managerUser.Id,
                        RoleId = clinicMgrRole.Id
                    };
                    await ctx.AddAsync(mgrUser);
                    var clinic1 = new Clinic()
                    {
                        EmailAddress = mgr1Email,
                        StreetAddress = "203 Wolseley Street\nBelfast",
                        Telephone = "90901",
                        ManagerUserId = managerUser.Id,
                        Rooms = new List<Room>()
                        {
                            new() { Description = "", Title = "Surgery Room 1", PricePerUse = 38.8},
                            new() { Description = "", Title = "Surgery Room 2", PricePerUse = 40.0}
                        }
                    };
                    await ctx.AddAsync(clinic1);
                    await ctx.SaveChangesAsync();
                    
                    
                    // BEGIN T2 PRACTICE
                    var t2 = new Tenancy()
                    {
                        OrgName = "Tom's Rhinoplasty",
                        ContactEmail = "bastardo@primusville.com",
                        LicenceStatus = Enums.LicenceStatus.Active,
                        RegistrationDate = DateTime.UtcNow,
                        OrgTagline = "never smelt better.",
                        Type = TenancyType.Practice
                    };
                    
                    await ctx.AddAsync(t2);
                    await ctx.SaveChangesAsync();
                    tenancySrv.SetTenancyId(t2.Id);
                    
                    var t2ClientEmail = "mrg@sphigh.com";
                    var t2Client= new Client()
                    {
                        Forename = "herbert",
                        Surname = "garrison",
                        Title = Enums.Title.Mr,
                        Address = "south park",
                        DateOfBirth = new DateOnly(1962, 07, 08),
                        Img = "/samples/herbertg.jpg",
                        Telephone = "28228282",
                        Tenancy = t2,
                        User =  new() {
                            Email = t2ClientEmail,
                            NormalizedEmail = t2ClientEmail.ToUpper(),
                            UserName = t2ClientEmail,
                            NormalizedUserName = t2ClientEmail.ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = true,
                            PhoneNumber = "2828282",
                            PhoneNumberConfirmed = true,
                            Tenancy = t2
                        }
                    };
                    t2Client.User.PasswordHash = passwordHasher.HashPassword(t2Client.User, "misterslave");
                    
                    var selfBookingT2 = new Setting()
                    {
                        Key = "SelfBookingEnabled",
                        Value = "False"
                    };
                    ctx.Add(selfBookingT2);
                    
                    var client3Email = "les@primusville.com";
                    var t2Prac = new Practitioner()
                    {
                        Forename = "Les",
                        Surname = "Claypool",
                        Title = Enums.Title.Mr,
                        DateOfBirth = new DateOnly(1968, 07, 08),
                        Img = "/samples/les.jpg",
                        Tenancy = t2,
                        User = new() {
                            Email = client3Email,
                            NormalizedEmail = client3Email.ToUpper(),
                            UserName = client3Email,
                            NormalizedUserName = client3Email.ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = true,
                            PhoneNumber = "98989",
                            PhoneNumberConfirmed = true,
                            Tenancy = t2
                        }
                    };
                    t2Prac.User.PasswordHash = passwordHasher.HashPassword(t2Prac.User, "johnthefisherman");
                    
                    var t2practice = new Practice()
                    {
                        EmailAddress = "nose@jobs.com",
                        Description = "Tom's Rhinoplasty",
                        Telephone = "0898 333 201",
                        Clients = new List<Client>() {t2Client},
                        Practitioners = new List<Practitioner>() {t2Prac},
                        Tenancy = t2
                    };
                    
                    var t2treatment = new Treatment()
                    {
                        Title = "nose job",
                        Description = "Never smelt better",
                        Cost = 8.99,
                        Tenancy = t2
                    };
                    
                    ctx.Add(t2practice);
                    ctx.Add(t2treatment);
                    
                    var t2ClientPracs = new List<ClientPractitioner>()
                    {
                        new () {Client = t2Client, Practitioner = t2Prac, Tenancy = t2},
                    };

                    await ctx.AddRangeAsync(t2ClientPracs);
                    await ctx.SaveChangesAsync();

                    t2practice.LeadPractitioner = t2Prac;

                    var t2Roles = new List<IdentityUserRole<string>>
                    {
                        new () { UserId = t2Prac.User.Id, RoleId = pracRole.Id },
                        new () { UserId = t2Client.User.Id, RoleId = clientRole.Id },
                    };
                    
                    await ctx.AddRangeAsync(t2Roles);
                    await ctx.SaveChangesAsync();
                    
                    // END T2
                }
            }
        }
    }
}

