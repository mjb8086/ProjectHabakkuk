/******************************
* HBK Database Context
* Bridges code to SQL.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database;

public class HbkContext : DbContext
{
    public HbkContext(DbContextOptions<HbkContext> options)
        : base(options)
    {
    }

    public DbSet<Practitioner> Practitioner { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*
        modelBuilder.Entity<PractitionerSpecialty>()
            .HasOne(ps => ps.Practitioner)
            .WithMany(ps => ps.PractitionerSpecialties)
            .HasForeignKey(ps => ps.PracIdx)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PractitionerSpecialty>()
            .HasOne(ps => ps.Specialty)
            .WithMany(ps => ps.PractitionerSpecialties)
            .HasForeignKey(ps => ps.SpecIdx)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Review>()
            .HasOne(p => p.Practitioner)
            .WithMany(b => b.Reviews);

*/
        // All models will have the date created ts.
        modelBuilder.Entity<HbkBaseEntity>()
            .Property(b => b.DateCreated)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

    }
}