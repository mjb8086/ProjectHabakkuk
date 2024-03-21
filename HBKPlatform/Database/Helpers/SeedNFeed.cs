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
                        OrgName = "Hill Valley Medical, Inc",
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
                    tenancySrv.SetTenancyId(t.Id);
                    
                    var user1 = new User()
                    {
                        Email = "outoftime@hillvalley.com",
                        NormalizedEmail = "outoftime@hillvalley.com".ToUpper(),
                        UserName = "outoftime@hillvalley.com",
                        NormalizedUserName = "outoftime@hillvalley.com".ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = true,
                        PhoneNumber = "0898 333 201",
                        PhoneNumberConfirmed = true,
                        Tenancy = t
                    };
                    user1.PasswordHash = passwordHasher.HashPassword(user1, "88milesperhour");
                    
                    var prac1 = new Practitioner()
                    {
                        Forename = "Emmett",
                        Surname = "Brown",
                        Title = Enums.Title.Dr,
                        ClientBio = "inventor of the flux capacitor",
                        Location = "hill valley",
                        DateOfBirth = new DateOnly(1932, 07, 08),
                        Img = new string("samples/brown.jpg"),
                        User = user1,
                        GmcNumber = "foo",
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
                        Img = new string("samples/second.jpg"),
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
                    prac2.User.PasswordHash = passwordHasher.HashPassword(prac2.User, "88milesperhour");

                    var client1Email = "mmf@hillvalleyhigh.com";
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
                    client1User.PasswordHash = passwordHasher.HashPassword(client1User, "toodamnloud");

                    var setting = new Setting()
                    {
                        Key = "SelfBookingEnabled",
                        Value = "True"
                    };
                    ctx.Add(setting);
                    
                    var client1 = new Client()
                    {
                        Forename = "Marty",
                        Surname = "McFly",
                        Title = Enums.Title.Mr,
                        Address = "a garage",
                        DateOfBirth = new DateOnly(1962, 07, 08),
                        Img = new string("samples/marty.jpg"),
                        Telephone = "999",
                        User = client1User,
                        Tenancy = t
                    };
                    
                    var client2Email = "biff@hillvalleyhigh.com";
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
                        Forename = "Biff",
                        Surname = "Tenant",
                        Title = Enums.Title.Mr,
                        Address = "A big house somewhere",
                        DateOfBirth = new DateOnly(1962, 07, 08),
                        Img = "samples/biff.jpg",
                        Telephone = "299",
                        User = client2User,
                        Tenancy = t
                    };
                    
                    var client3Email = "les@primus.com";
                    var client3User = new User()
                    {
                        Email = client3Email,
                        NormalizedEmail = client3Email.ToUpper(),
                        UserName = client3Email,
                        NormalizedUserName = client3Email.ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = true,
                        PhoneNumber = "98989",
                        PhoneNumberConfirmed = true,
                        Tenancy = t
                    };
                    client3User.PasswordHash = passwordHasher.HashPassword(client1User, "johnthefisherman");
                    
                    var client3 = new Client()
                    {
                        Forename = "Les",
                        Surname = "Claypool",
                        Title = Enums.Title.Mr,
                        Address = "Rancho Relaxo",
                        DateOfBirth = new DateOnly(1968, 07, 08),
                        Img = "samples/les.jpg",
                        Telephone = "919191",
                        User = client3User,
                        Tenancy = t
                    };
                    
                    var practice = new Practice()
                    {
                        EmailAddress = "foo@bar.com",
                        Description = "Hill Valley Practice",
                        Telephone = "0898 333 201",
                        Clients = new List<Client>() {client1, client2, client3},
                        Practitioners = new List<Practitioner>() {prac1, prac2},
                        Tenancy = t
                    };

                    ctx.Add(practice);
                    ctx.SaveChanges();

                    practice.LeadPractitioner = prac1;
                    ctx.SaveChanges();

                    var clientPracs = new List<ClientPractitioner>()
                    {
                        new () {Client = client1, Practitioner = prac1, Tenancy = t},
                        new () {Client = client2, Practitioner = prac1, Tenancy = t},
                        new () {Client = client3, Practitioner = prac2, Tenancy = t},
                    };

                    ctx.AddRange(clientPracs);

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
                        MessageBody = "lost the plutonium sorry", Tenancy = t
                    });
                    conversation.Add(new ()
                    {
                        ClientId = client1.Id, PractitionerId = prac1.Id, MessageOrigin = Enums.MessageOrigin.Practitioner,
                        MessageBody = "ah bollocks", Tenancy = t
                    });
                    conversation.Add(new ()
                    {
                        ClientId = client2.Id, PractitionerId = prac1.Id,  MessageOrigin = Enums.MessageOrigin.Practitioner,
                        MessageBody = "don't steal that almanac you tool", Tenancy = t
                    });
                    ctx.AddRange(conversation);

                    var clientRecord1 = new ClientRecord()
                    {
                        Client = client1, RecordVisibility = Enums.RecordVisibility.ClientAndPrac,
                        Title = "Bad news for the bowels", NoteBody = "bother shifting the goods", Practitioner = prac1, Tenancy = t
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

                    var timeslots = TimeslotHelper.GenerateDefaultTimeslots(t);
                    
                    ctx.AddRange(timeslots);
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
                            Timeslot = timeslots[2],
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
                    
                    var clinicTenancy = new Tenancy() {
                        OrgName = "The Coachman",
                        LicenceStatus = Enums.LicenceStatus.Active,
                        Type = TenancyType.Clinic
                    };
                    await ctx.AddAsync(clinicTenancy);
                    await ctx.SaveChangesAsync();
                    tenancySrv.SetTenancyId(clinicTenancy.Id);
                    
                    var mgr1Email = "coach@btinternet.com";
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
                        OrgName = "The Coachman",
                        StreetAddress = "Broad Street\nMagherafelt",
                        Telephone = "1690",
                        EmailAddress = "coachm@btinternet.com",
                        ManagerUserId = managerUser.Id,
                        Rooms = new List<Room>()
                        {
                            new() { Description = "Pool room", Title = "Pool Room", PricePerUse = 8.8},
                            new() { Description = "Drinks and a jukebox", Title = "Front Bar", PricePerUse = 100.0},
                            new() { Description = "bring coat", Title = "Beer garden", PricePerUse = 3.0}
                        }
                    };
                    await ctx.AddAsync(clinic1);
                    await ctx.SaveChangesAsync();
                }
            }
        }
    }
}

