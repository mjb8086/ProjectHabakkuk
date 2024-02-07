using HBKPlatform.Globals;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database.Helpers
{
    
    // Sneed's
    public static class SeedNFeed //Formerly Chuck's
    {
        public const int DEFAULT_DURATION = 30;
        public static async Task Initialise(IServiceProvider provider, IPasswordHasher<User> passwordHasher)
        {
            using (var ctx = new ApplicationDbContext(
               provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var roleStore = new RoleStore<IdentityRole>(ctx);
                IdentityRole superAdminRole, pracRole, clientRole;

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
                    
                if (!ctx.Practitioners.Any() && !ctx.Clients.Any() && !ctx.Clinics.Any()) {
                    
                    var user1 = new User()
                    {
                        Email = "outoftime@hillvalley.com",
                        NormalizedEmail = "outoftime@hillvalley.com".ToUpper(),
                        UserName = "outoftime@hillvalley.com",
                        NormalizedUserName = "outoftime@hillvalley.com".ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "0898 333 201",
                        PhoneNumberConfirmed = true,
                    };
                    user1.PasswordHash = passwordHasher.HashPassword(user1, "88milesperhour");
                    
                    var prac1 = new Practitioner()
                    {
                        Forename = "Emmett",
                        Surname = "Brown",
                        Title = Enums.Title.Dr,
                        Bio = "inventor of the flux capacitor",
                        Location = "hill valley",
                        DateOfBirth = new DateTime(1932, 07, 08).ToUniversalTime(),
                        Img = new string("samples/brown.jpg"),
                        User = user1
                    };

                    var client1Email = "mmf@hillvalleyhigh.com";
                    var client1User = new User()
                    {
                        Email = client1Email,
                        NormalizedEmail = client1Email.ToUpper(),
                        UserName = client1Email,
                        NormalizedUserName = client1Email.ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "0898 333 201",
                        PhoneNumberConfirmed = true,
                    };
                    client1User.PasswordHash = passwordHasher.HashPassword(client1User, "toodamnloud");
                    
                    var client1 = new Client()
                    {
                        Forename = "Marty",
                        Surname = "McFly",
                        Title = Enums.Title.Mr,
                        Address = "a garage",
                        DateOfBirth = new DateOnly(1962, 07, 08),
                        Img = new string("samples/marty.jpg"),
                        Telephone = "999",
                        User = client1User
                    };
                    
                    var client2Email = "biff@hillvalleyhigh.com";
                    var client2User = new User()
                    {
                        Email = client2Email,
                        NormalizedEmail = client2Email.ToUpper(),
                        UserName = client2Email,
                        NormalizedUserName = client2Email.ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "0898 333 201",
                        PhoneNumberConfirmed = true,
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
                        User = client2User
                    };
                    
                    var clinic = new Clinic()
                    {
                        EmailAddress = "foo@bar.com",
                        LicenceStatus = Enums.LicenceStatus.Active,
                        OrgName = "Hill Valley Clinic",
                        OrgTagline = "Timely treatment or your time back.",
                        Telephone = "0898 333 201",
                        LeadPractitioner = prac1,
                        Clients = new List<Client>() {client1, client2},
                        RegistrationDate = DateTime.UtcNow
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
                    };
                    superUser.PasswordHash = passwordHasher.HashPassword(client1User, "changeme123");

                    ctx.Add(superUser);
                    ctx.Add(clinic);
                    ctx.SaveChanges();

                    var roles = new List<IdentityUserRole<string>>
                    {
                        new IdentityUserRole<string>() { UserId = user1.Id, RoleId = pracRole.Id },
                        new IdentityUserRole<string>() { UserId = client1User.Id, RoleId = clientRole.Id },
                        new IdentityUserRole<string>() { UserId = client2User.Id, RoleId = clientRole.Id },
                        new IdentityUserRole<string>() { UserId = superUser.Id, RoleId = superAdminRole.Id }
                    };
                    
                    ctx.AddRange(roles);

                    var conversation = new List<ClientMessage>();
                    conversation.Add(new ClientMessage()
                    {
                        ClientId = client1.Id, PractitionerId = prac1.Id, ClinicId = clinic.Id, MessageOrigin = Enums.MessageOrigin.Client,
                        MessageBody = "lost the plutonium sorry"
                    });
                    conversation.Add(new ClientMessage()
                    {
                        ClientId = client1.Id, PractitionerId = prac1.Id, ClinicId = clinic.Id, MessageOrigin = Enums.MessageOrigin.Practitioner,
                        MessageBody = "ah bollocks"
                    });
                    conversation.Add(new ClientMessage()
                    {
                        ClientId = client2.Id, PractitionerId = prac1.Id, ClinicId = clinic.Id, MessageOrigin = Enums.MessageOrigin.Practitioner,
                        MessageBody = "don't steal that almanac you tool"
                    });
                    ctx.AddRange(conversation);

                    var clientRecord1 = new ClientRecord()
                    {
                        Clinic = clinic, Client = client1, RecordVisibility = Enums.RecordVisibility.ClientAndPrac,
                        Title = "Bad news for the bowels", NoteBody = "bother shifting the goods"
                    };
                    ctx.Add(clientRecord1);

                    var treatment1 = new Treatment()
                    {
                        Clinic = clinic,
                        TreatmentRequestability = Enums.TreatmentRequestability.ClientAndPrac,
                        Title = "Checkup",
                        Description = "talk and pretend to do something",
                        Cost = 15.50
                    };
                    
                    var treatment2 = new Treatment()
                    {
                        Clinic = clinic,
                        TreatmentRequestability = Enums.TreatmentRequestability.PracOnly,
                        Title = "Prac Only Test",
                        Description = "no book for client",
                        Cost = 69.0
                    };

                    ctx.Add(treatment1);
                    ctx.Add(treatment2);

                    var timeslots = new List<Timeslot>();
                    var days = new [] {Enums.Day.Monday, Enums.Day.Tuesday, Enums.Day.Wednesday, Enums.Day.Thursday, Enums.Day.Friday, Enums.Day.Saturday, Enums.Day.Sunday};
                    foreach (var day in days)
                    {
                        var time = new TimeOnly(08, 00, 00);
                        var maxTime = new TimeOnly( 19, 00, 00);
                        
                        while (time <= maxTime)
                        {
                            timeslots.Add(new Timeslot() { Clinic = clinic, Description = $"{day} {time.ToShortTimeString()}", Day = day, Time = time, Duration = DEFAULT_DURATION});
                            time = time.AddMinutes(DEFAULT_DURATION);
                        }
                    }
                    
                    ctx.AddRange(timeslots);
                    ctx.SaveChanges();

                    var appointments = new[] { 
                        new Appointment()
                        {
                            Client = client1,
                            Clinic = clinic,
                            Practitioner = prac1,
                            Timeslot = timeslots[1],
                            Status = Enums.AppointmentStatus.Live,
                            Treatment = treatment1,
                            WeekNum = 4
                        },
                        new Appointment()
                        {
                            Client = client1,
                            Clinic = clinic,
                            Practitioner = prac1,
                            Timeslot = timeslots[2],
                            Status = Enums.AppointmentStatus.CancelledByClient,
                            CancellationReason = "Someone else does it cheaper, sorry",
                            Treatment = treatment1,
                            WeekNum = 30
                        },
                        new Appointment()
                        {
                            Client = client1,
                            Clinic = clinic,
                            Practitioner = prac1,
                            Timeslot = timeslots[2],
                            Status = Enums.AppointmentStatus.CancelledByPractitioner,
                            CancellationReason = "bollocks",
                            Treatment = treatment2,
                            WeekNum = 30
                        }
                        
                    };
                    
                    ctx.AddRange(appointments);

                    var startDate = new Setting()
                    {
                        Key = "DbStartDate",
                        Value = "2024-01-01",
                        Clinic = clinic
                    };
                    ctx.Add(startDate);

                    var ta = new List<TimeslotAvailability>()
                    {
                        new TimeslotAvailability()
                        {
                            Timeslot = timeslots[100],
                            Practitioner = prac1,
                            Availability = Enums.TimeslotAvailability.Unavailable,
                            WeekNum = 20
                        },
                        new TimeslotAvailability()
                        {
                            Timeslot = timeslots[101],
                            Practitioner = prac1,
                            Availability = Enums.TimeslotAvailability.Unavailable,
                            WeekNum = 20
                        },
                        new TimeslotAvailability()
                        {
                            Timeslot = timeslots[102],
                            Practitioner = prac1,
                            Availability = Enums.TimeslotAvailability.Unavailable,
                            WeekNum = 20
                        },
                        new TimeslotAvailability()
                        {
                            Timeslot = timeslots[104],
                            Practitioner = prac1,
                            Availability = Enums.TimeslotAvailability.Available,
                            WeekNum = 20
                        },
                    };
                    ctx.AddRange(ta);
                    ctx.SaveChanges();
                    
                }

            }
        }
    }
}

