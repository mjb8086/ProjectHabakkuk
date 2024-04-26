using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Events;
using Hangfire;
using Hangfire.PostgreSql;

using HBKPlatform.Areas.Account;
using HBKPlatform.Database;
using HBKPlatform.Database.Helpers;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Middleware;
using HBKPlatform.Repository;
using HBKPlatform.Repository.Implementation;
using HBKPlatform.Services;
using HBKPlatform.Services.Implementation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Vite.AspNetCore;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    // BEGIN Builder.
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    Console.WriteLine($"NowDoctor Ltd. Presents:\n{Consts.HBK_NAME}\nVersion {Consts.VERSION}. Starting up...");
    
    builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Lockout.MaxFailedAccessAttempts = DefaultSettings.LOCKOUT_MAX_ATTEMPTS;
        })
        .AddRoleManager<RoleManager<IdentityRole>>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultUI()
        .AddDefaultTokenProviders();

    // To ensure custom claims (PracticeId, PracId, etc) are added to new identity when principal is refreshed.
    builder.Services.ConfigureOptions<ConfigureSecurityStampOptions>();

    // Scoped - created once per HTTP request. Use for database because there may be multiple calls in a web service.
    builder.Services.AddScoped<IPractitionerRepository, PractitionerRepository>();
    builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
    builder.Services.AddScoped<IClientRepository, ClientRepository>();
    builder.Services.AddScoped<IClientMessageRepository, ClientMessageRepository>();
    builder.Services.AddScoped<IRecordRepository, RecordRepository>();
    builder.Services.AddScoped<ITreatmentRepository, TreatmentRepository>();
    builder.Services.AddScoped<ITimeslotRepository, TimeslotRepository>();
    builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
    builder.Services.AddScoped<ISettingRepository, SettingRepository>();
    builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ITenancyService, TenancyService>();
    builder.Services.AddScoped<IMcpRepository, McpRepository>();
    builder.Services.AddScoped<IRoomRepository, RoomRepository>();
    builder.Services.AddScoped<IRoomReservationRepository, RoomReservationRepository>();

    builder.Services.AddScoped<TenancyMiddleware>();
    builder.Services.AddScoped<CentralScrutinizerMiddleware>();

    // Also use scoped for the cache and user service - both are utilised regularly
    builder.Services.AddScoped<ICacheService, CacheService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ISecurityService, SecurityService>();

    // Transient - created each time it is required. Use on web services, because they will typically serve one action
    // to the controller.
    builder.Services.AddTransient<IPracticeService, PracticeService>();
    builder.Services.AddTransient<IClientMessagingService, ClientMessagingService>();
    builder.Services.AddTransient<IClientRecordService, ClientRecordService>();
    builder.Services.AddTransient<ITreatmentService, TreatmentService>();
    builder.Services.AddTransient<IBookingService, BookingService>();
    builder.Services.AddTransient<IConfigurationService, ConfigurationService>();
    builder.Services.AddTransient<IClientDetailsService, ClientDetailsService>();
    builder.Services.AddTransient<IAvailabilityManagementService, AvailabilityManagementService>();
    builder.Services.AddTransient<IMcpService, McpService>();
    builder.Services.AddTransient<IRoomService, RoomService>(); // yes, we come with room service. 100% satisfaction guarantee
    builder.Services.AddTransient<IRoomReservationService, RoomReservationService>();
    builder.Services.AddTransient<ITimeslotService, TimeslotService>();

    // Singleton - created once at startup. Use only where immutability or heftiness is likely. i.e. a distributed cache.
    builder.Services.AddSingleton<IDateTimeWrapper, DateTimeWrapper>();
    builder.Services.AddSingleton<ICentralScrutinizerService, CentralScrutinizerService>();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("HbkContext") ??
                throw new InvalidOperationException("Connection string is invalid.") );
            if (builder.Environment.IsDevelopment()) { options.EnableSensitiveDataLogging(); }
        }
    );

    // Routing config - enable lowercase URLs
    builder.Services.AddRouting(options => options.LowercaseUrls = true);

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddViteServices();
    
    // Add Hangfire services. This facilitates background tasks.
    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(x =>
        {
            x.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HangfireConnection"));
        }));

    // Add the processing server as IHostedService
    builder.Services.AddHangfireServer();
    
    var mvcBuilder = builder.Services.AddRazorPages();

    if (builder.Environment.IsDevelopment())
    {
        mvcBuilder.AddRazorRuntimeCompilation();
    }

    // END builder, create the webapp instance...
    var app = builder.Build();
    
    // Autodeploy and Seed DB with sample data and admin user if necessary.
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        var services = scope.ServiceProvider;
        await SeedNFeed.Initialise(services, new PasswordHasher<User>());
    }

    app.UseStaticFiles();
    app.UseSerilogRequestLogging();
    app.UseRouting();

    // Register middleware
    if (builder.Environment.IsDevelopment())
    {
        // Use header forwarding to Nginx when in production.
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
    }

    app.UseMiddleware<TenancyMiddleware>();
    app.UseMiddleware<CentralScrutinizerMiddleware>();

    app.UseAuthorization();

    // Include mappings to pass all SPA routes to the right endpoints.
    app.MapControllerRoute(name: "MyND", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
    app.MapControllerRoute(name: "Client", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
    // Two alternatives, neither are in use because neither seem to do anything.
/*    app.MapControllerRoute(
        name: "MyNDSpa",
        pattern: "/mynd/reception/newui/{*url}",
        defaults: new {area="MyND", controller = "Reception", action = "NewUI"}
    );*/
    app.MapRazorPages();

    /*
    var options = new RewriteOptions()
        .AddRewrite("^/mynd/reception/newui/.*", "/mynd/reception/newui/", skipRemainingRules: true)
        .AddRewrite("^/clinic/newui/.*", "/clinic/newui/", skipRemainingRules: true);
    app.UseRewriter(options);
*/

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment()) // configure for dev environment, enable all routes listing 
    {
        app.UseStatusCodePagesWithReExecute("/Home/ErrorDev", "?statusCode={0}");
        app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
            string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)).ToLower());
        
        app.UseViteDevelopmentServer(true);
    }
    else // configure production
    {
        app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        app.UseHttpsRedirection();
        builder.WebHost.UseUrls("http://*:5000");
    }
    
    
    Log.Information("HBKPlatform startup complete.");

    app.Run();
    
    // Register background tasks.
    RecurringJob.AddOrUpdate<ICentralScrutinizerService>("pruneactive", css => css.PruneActiveUsers(), "1 * * * *");

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
