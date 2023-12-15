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
            using (var ctx = new ApplicationDbContext(
               provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var roleStore = new RoleStore<IdentityRole>(ctx);
                IdentityRole sysOp, pracRole, clientRole;

                if ((sysOp = ctx.Roles.FirstOrDefault(r => r.Name == "SysOp")) == null)
                {
                    sysOp = new IdentityRole() { Name = "SysOp", NormalizedName = "SysOp".ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString()};
                    await roleStore.CreateAsync(sysOp);
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
                    
                if (!ctx.Practitioners.Any() && !ctx.Clinics.Any()) {
                    
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
                        Title = Title.Dr,
                        Bio = "inventor of the flux capacitor",
                        Location = "hill valley",
                        DateOfBirth = new DateTime(1932, 07, 08).ToUniversalTime(),
                        Img = new string("samples/brown.jpg"),
                        User = user1
                    };

                    var user2Email = "mmf@hillvalleyhigh.com";
                    var user2 = new User()
                    {
                        Email = user2Email,
                        NormalizedEmail = user2Email.ToUpper(),
                        UserName = user2Email,
                        NormalizedUserName = user2Email.ToUpper(),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "0898 333 201",
                        PhoneNumberConfirmed = true,
                    };
                    user2.PasswordHash = passwordHasher.HashPassword(user2, "toodamnloud");
                    
                    var client1 = new Client()
                    {
                        Forename = "Marty",
                        Surname = "McFly",
                        Title = Title.Mr,
                        Address = "Unknown",
                        DateOfBirth = new DateTime(1962, 07, 08).ToUniversalTime(),
                        Img = new string("samples/brown.jpg"),
                        Telephone = "999",
                        User = user2
                    };
                    
                    ctx.AddRange(
                        new Clinic()
                        {
                            EmailAddress = "foo@bar.com",
                            LicenceStatus = LicenceStatus.Active,
                            OrgName = "Hill Valley Clinic",
                            OrgTagline = "Timely treatment or your time back.",
                            Telephone = "0898 333 201",
                            Practitioner = prac1,
                            Clients = new List<Client>() {client1}
                        }
                    );
                    ctx.SaveChanges();
                    
                    var pracUserRole = new IdentityUserRole<string>() { UserId = user1.Id, RoleId = pracRole.Id };
                    var clientUserRole = new IdentityUserRole<string>() { UserId = user2.Id, RoleId = clientRole.Id };
                    ctx.Add(pracUserRole);
                    ctx.Add(clientUserRole);
                }

                ctx.SaveChanges();
            }
        }
    }
}

