/******************************
* HBK Database Context
* Bridges code to SQL.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using System.Text.RegularExpressions;
using HBKPlatform.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database;

public class HbkContext : DbContext
{
    public HbkContext(DbContextOptions<HbkContext> options)
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
        // modelBuilder.Entity<Client>().ToTable("clients");
        // modelBuilder.Entity<ClientMessage>().ToTable("client_messages");
        // modelBuilder.Entity<Clinic>().ToTable("clinics");
        // modelBuilder.Entity<ClinicHomepage>().ToTable("clinic_homepage");
        // modelBuilder.Entity<Practitioner>().ToTable("practitioner");
        
        foreach(var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Replace table names
            entity.SetTableName(entity.GetTableName().ToSnakeCase());

            // Replace column names            
            foreach(var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToSnakeCase());
            }

            foreach(var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ToSnakeCase());
            }

            foreach(var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
            }

            foreach(var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
            }
        }
    }
    
    

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