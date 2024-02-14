/******************************
* HBK Database Context
* Bridges code to SQL.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using HBKPlatform.Database.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database;

public class ApplicationDbContext : IdentityDbContext<User>

{
    private IHttpContextAccessor _httpCtx;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpCtx)
        : base(options)
    {
        _httpCtx = httpCtx;
    }

    // Define tables
    public DbSet<Appointment> Appointments { get; set; } = default!;
    public DbSet<Client> Clients { get; set; } = default!;
    public DbSet<ClientMessage> ClientMessages { get; set; } = default!;
    public DbSet<ClientRecord> ClientRecords { get; set; } = default!;
    public DbSet<Clinic> Clinics { get; set; } = default!;
    public DbSet<ClinicHomepage> ClinicHomepages { get; set; } = default!;
    public DbSet<Practitioner> Practitioners { get; set; } = default!;
    public DbSet<Timeslot> Timeslots { get; set; } = default!;
    public DbSet<TimeslotAvailability> TimeslotAvailabilities { get; set; } = default!;
    public DbSet<Treatment> Treatments { get; set; } = default!;
    public DbSet<Setting> Settings { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Manual relationships
        // Configure one-to-many relationship between Clinic and Practitioner
        modelBuilder.Entity<Practitioner>()
            .HasOne(p => p.Clinic)
            .WithMany(c => c.Practitioners)
            .HasForeignKey(p => p.ClinicId);

        // Configure one-to-one relationship between Clinic and LeadPractitioner
        modelBuilder.Entity<Clinic>()
            .HasOne(c => c.LeadPractitioner)
            .WithOne()
            .HasForeignKey<Clinic>(c => c.LeadPractitionerId);

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
        
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
        modelBuilder.Entity<IdentityRole>().ToTable("application_roles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
    }
    
    /// <summary>
    /// Set snake case on other entities.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSnakeCaseNamingConvention();

    /// <summary>
    /// Automatically update DateModified for all entities that inherit from BaseEntity.
    /// </summary>
    void UpdateEntries()
    {
        ChangeTracker.DetectChanges();

        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);
        var createdEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added);

        // is the HTTP context available
        string? userName = null;
        if (_httpCtx.HttpContext != null)
        {
            var user = _httpCtx.HttpContext.User;
            userName = user?.Identity?.Name;
        }

        foreach (var entry in modifiedEntries)
        {
            if (entry.Entity is HbkBaseEntity entity)
            {
                entity.DateModified = DateTime.UtcNow;
                entity.ModifyActioner = userName;
            }
        }
        foreach (var entry in createdEntries)
        {
            if (entry.Entity is HbkBaseEntity entity)
            {
                entity.CreateActioner = userName;
            }
        }
    }
    
    public override int SaveChanges()
    {
        UpdateEntries();
        return base.SaveChanges();
    }
    
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateEntries();
        return base.SaveChangesAsync(cancellationToken);
    }
}