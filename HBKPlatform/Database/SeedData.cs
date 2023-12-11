using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database
{
    public static class SeedData //Formerly Chuck's
    {
        public static void Initialise(IServiceProvider provider)
        {
            using (var ctx = new ApplicationDbContext(
               provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (!ctx.Practitioners.Any() && !ctx.Clinics.Any()) {
                    var prac1 = new Practitioner()
                    {
                        Name = "Emmert Brown",
                        Title = Title.Dr,
                        Bio = "flux capacator",
                        Location = "hill valley",
                        DOB = new DateTime(1932, 07, 08).ToUniversalTime(),
                        Img = new string("samples/brown.jpg")
                    };
                
                    ctx.AddRange(
                        new Clinic()
                        {
                            EmailAddress = "foo@bar.com",
                            LicenceStatus = LicenceStatus.Active,
                            OrgName = "Sample Clinic",
                            OrgTagline = "Doing the best we can.",
                            Telephone = "0898 333 201",
                            Practitioner = prac1
                        }
                    );
                
                }

                if (false && !ctx.Practitioners.Any())
                {
                    ctx.AddRange(
                        new Practitioner
                        {
                            Name = "Emmert Brown",
                            Title = Title.Dr,
                            Bio = "flux capacator",
                            Location = "hill valley",
                            DOB = new DateTime(1932, 07, 08).ToUniversalTime(),
                            Img = new string("samples/brown.jpg")
                        },
                        new Practitioner
                        {
                            Name = "Dre",
                            Title = Title.Dr,
                            Bio = "not forgotten",
                            Location = "east side",
                            DOB = new DateTime(1972, 06, 19).ToUniversalTime(),
                            Img = new string("samples/dre.jpg")
                        },
                        new Practitioner
                        {
                            Name = "Foo",
                            Title = Title.Dr,
                            Location = "bar",
                            Bio = "baz",
                            DOB = new DateTime(2030, 12, 12).ToUniversalTime(),
                            Img = new string("samples/foo.jpg")
                        },
                        new Practitioner
                        {
                            Name = "Bombay",
                            Title = Title.Dr,
                            Location = "calcutta",
                            Bio = "rice and curry in a hurry",
                            DOB = new DateTime(1979, 09, 23).ToUniversalTime(),
                            Img = new string("samples/bombay.jpg")
                        },
                        new Practitioner
                        {
                            Name = "Evil",
                            Title = Title.Dr,
                            Location = "space",
                            Bio = "I work for One Million Dollars, but you get the world",
                            DOB = new DateTime(1945, 08, 01).ToUniversalTime(),
                            Img = new string("samples/evil.jpg")
                        },
                        new Practitioner
                        {
                            Name = "william mccrea",
                            Title = Title.Dr,
                            Location = "magherafelt",
                            Bio = "the true gosepl says buy my albums (kjv only)++",
                            DOB = new DateTime(1901, 01, 07).ToUniversalTime(),
                            Img = new string("samples/mccrea.jpg")
                        },
                        new Practitioner
                        {
                            Name = "Moley",
                            Title = Title.Dr,
                            Location = "america",
                            Bio = "I have watched many hours of instructional anatomic videos. You are safe alone with me.",
                            DOB = new DateTime(1961, 01, 17).ToUniversalTime(),
                            Img = new string("samples/pills.jpg")
                        },
                        new Practitioner
                        {
                            Name = "Doolittle",
                            Title = Title.Dr,
                            Location = "america",
                            Bio = "doesn't know anything about medicine but talks to animals",
                            DOB = new DateTime(1961, 01, 17).ToUniversalTime(),
                            Img = new string("samples/doolittle.jpg")
                        },
                        new Practitioner
                        {
                            Name = "Hippocrates",
                            Title = Title.Dr,
                            Location = "Ancient Greece",
                            Bio = "I invented this. Ask me anything and I will know.",
                            DOB = new DateTime(1961, 01, 17).ToUniversalTime(),
                            Img = new string("samples/hippo.jpg")
                        }
                    );
                }
                ctx.SaveChanges();
            }
        }
    }
}

