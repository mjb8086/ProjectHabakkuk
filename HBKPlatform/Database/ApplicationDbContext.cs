/******************************
* HBK Database Context
* Bridges code to SQL.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using System.Security.Claims;
using HBKPlatform.Database.Helpers;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>

    {
        private IHttpContextAccessor _httpCtx;
        private ITenancyService _tenancySrv;
    
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpCtx, ITenancyService tenancySrv)
            : base(options)
        {
            _httpCtx = httpCtx;
            _tenancySrv = tenancySrv;
        }

        // Define tables
        public DbSet<Appointment> Appointments { get; set; } = default!;
        public DbSet<Client> Clients { get; set; } = default!;
        public DbSet<ClientMessage> ClientMessages { get; set; } = default!;
        public DbSet<ClientRecord> ClientRecords { get; set; } = default!;
        public DbSet<Practice> Practices { get; set; } = default!;
        public DbSet<Practitioner> Practitioners { get; set; } = default!;
        public DbSet<Tenancy> Tenancies { get; set; } = default!;
        public DbSet<Timeslot> Timeslots { get; set; } = default!;
        public DbSet<TimeslotAvailability> TimeslotAvailabilities { get; set; } = default!;
        public DbSet<Treatment> Treatments { get; set; } = default!;
        public DbSet<Setting> Settings { get; set; } = default!;
        public DbSet<ClientPractitioner> ClientPractitioners { get; set; } = default!;
        public DbSet<Room> Rooms { get; set; } = default!;
        public DbSet<Attribute> Attributes { get; set; } = default!;
        public DbSet<RoomAttribute> RoomAttributes { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        
            // Set query filter for tenancy
            modelBuilder.Entity<HbkBaseEntity>().HasQueryFilter(e => e.TenancyId == _tenancySrv.TenancyId);
            modelBuilder.Entity<ClientPractitioner>().HasQueryFilter(e => e.TenancyId == _tenancySrv.TenancyId);
        
            // Manual relationships
            // Configure one-to-many relationship between Practice and Practitioner
            modelBuilder.Entity<Practitioner>()
                .HasOne(p => p.Practice)
                .WithMany(c => c.Practitioners)
                .HasForeignKey(p => p.PracticeId);

            // Configure one-to-one relationship between Practice and LeadPractitioner
            modelBuilder.Entity<Practice>()
                .HasOne(c => c.LeadPractitioner)
                .WithOne()
                .HasForeignKey<Practice>(c => c.LeadPractitionerId);

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
        /// Set snake case on all entities.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        /// <summary>
        /// Automatically update DateModified for all entities that inherit from BaseEntity.
        /// Set user ID and tenancy ID on create/update actions.
        /// </summary>
        void ProcessEntities()
        {
            ChangeTracker.DetectChanges();

            var modifiedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);
            var createdEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added);

            // is the HTTP context available
            string? userId = null;
            if (_httpCtx.HttpContext != null)
            {
                userId = _httpCtx.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity is HbkBaseEntity entity)
                {
                    entity.DateModified = DateTime.UtcNow;
                    entity.ModifyActioner =  userId;
                }
            }
            foreach (var entry in createdEntries)
            {
                if (entry.Entity is HbkBaseEntity entity)
                {
                    entity.CreateActioner =  userId;
                    entity.TenancyId = _tenancySrv.TenancyId;
                }
            }
        }

        public override int SaveChanges()
        {
            ProcessEntities();
            return base.SaveChanges();
        }
    
    
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ProcessEntities();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}