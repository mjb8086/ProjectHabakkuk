/******************************
* HBK Database Context
* Bridges code to SQL.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using HBKPlatform.Areas.Identity.Data;
using HBKPlatform.Database.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database;

public class ApplicationDbContext : IdentityDbContext<User>

{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Define tables
    public DbSet<Practitioner> Practitioners { get; set; } = default!;
    public DbSet<Client> Clients { get; set; } = default!;
    public DbSet<ClientMessage> ClientMessages { get; set; } = default!;
    public DbSet<Clinic> Clinics { get; set; } = default!;
    public DbSet<ClinicHomepage> ClinicHomepages { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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
        // Ensure inherited entities are flattened into one table.
        modelBuilder.Entity<HbkBaseEntity>().UseTpcMappingStrategy();

        // All models will have the date created ts.
        modelBuilder.Entity<HbkBaseEntity>()
            .Property(b => b.DateCreated)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Make Id columns auto increment.
        modelBuilder.UseIdentityAlwaysColumns();
        
        // Name tables in snake case.
        modelBuilder.NameModelEntitiesInSnakeCase();
         
    }
    
    /// <summary>
    /// Set snake case on other entities.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSnakeCaseNamingConvention();

    /// <summary>
    /// Automatically update DateModified for all entities that inherit from BaseEntity.
    /// </summary>
    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();

        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is HbkBaseEntity entity)
            {
                entity.DateModified = DateTime.UtcNow;
            }
        }

        return base.SaveChanges();
    }
}