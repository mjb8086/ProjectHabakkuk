using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

using Serilog;
using Serilog.Events;
using Hangfire;
//using Hangfire.PostgreSql;
using Hangfire.InMemory;
using Npgsql;
using Vite.AspNetCore;

using Hbk.Common.Helpers;
using Hbk.Common.Services;
using Hbk.Common.Services.Implementation;
using Hbk.Database;
using Hbk.Database.Helpers;
using Hbk.Platform.Areas.Account;
using Hbk.Platform.Defaults;
using Hbk.Platform.Middleware;
using Hbk.Platform.Repository;
using Hbk.Platform.Repository.Implementation;
using Hbk.Platform.Services;
using Hbk.Platform.Services.Implementation;



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
//        .AddDefaultUI()
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
    builder.Services.AddTransient<IReceptionService, ReceptionService>();

    // Singleton - created once at startup. Use only where immutability or heftiness is likely. i.e. a distributed cache.
    builder.Services.AddSingleton<IDateTimeWrapper, DateTimeWrapper>();
    builder.Services.AddSingleton<ICentralScrutinizerService, CentralScrutinizerService>();

    var useInMemoryDatabase = builder.Environment.IsDevelopment() &&
                              builder.Configuration.GetValue<bool>("Database:UseInMemory");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (useInMemoryDatabase)
            {
                options.UseInMemoryDatabase(
                    builder.Configuration.GetValue<string>("Database:InMemoryDatabaseName") ?? "HbkInMemory");
            }
            else
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("HbkContext") ??
                    throw new InvalidOperationException("Connection string is invalid.") );
            }

            if (builder.Environment.IsDevelopment()) { options.EnableSensitiveDataLogging(); }
        }
    );

    // Routing config - enable lowercase URLs
    builder.Services.AddRouting(options => options.LowercaseUrls = true);

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddViteServices();

    if (!useInMemoryDatabase)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder( builder.Configuration.GetConnectionString("HbkContext") );
        connectionStringBuilder.Database = "postgres";

        using var connection = new NpgsqlConnection(connectionStringBuilder.ToString());
        connection.Open();

        try
        {
            using var command = new NpgsqlCommand($"CREATE DATABASE hangfire;", connection);
            command.ExecuteNonQuery();
        }
        catch
        {
            // who cares if it already exists, anything more serious will throw again anyway
        }

    }
    
    // Add Hangfire services. This facilitates background tasks.
    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage(new InMemoryStorageOptions
        {
            MaxExpirationTime = TimeSpan.FromHours(3) // Default value, we can also set it to `null` to disable.
        }));
        // {
        //     x.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HangfireConnection"));
        // }));

    
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
        if (useInMemoryDatabase)
        {
            db.Database.EnsureCreated();
        }
        else
        {
            db.Database.Migrate();
        }

        var services = scope.ServiceProvider;
        await SeedNFeed.Initialise(services, new PasswordHasher<User>());
    }

    app.UseStaticFiles();
    app.UseSerilogRequestLogging();
    app.UseRouting();

    if (builder.Environment.IsDevelopment())
    {
        // Use header forwarding to Nginx when in production.
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
    }

    // Register middleware
    app.UseMiddleware<TenancyMiddleware>();
    app.UseMiddleware<CentralScrutinizerMiddleware>();

    app.UseAuthorization();

    // Configure the HTTP request pipeline.
    // Include mappings to pass all SPA routes to the right endpoints.
    app.MapControllerRoute(name: "MyND", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
    app.MapControllerRoute(name: "Client", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
    // Rewrite the SPA routes
    app.MapControllerRoute( name: "MyNDSpa", pattern: "/mynd/ui/{*url}", 
        defaults: new {area="MyND", controller = "Ui", action = "Index"} );
    app.MapControllerRoute( name: "ClientSpa", pattern: "/client/ui/{*url}", 
        defaults: new {area="Client", controller = "Ui", action = "Index"} );
    
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();
    app.MapControllers(); // will fill in routes as declared in decorators - use these for the API
    
    app.UseWebSockets();

    /*
    var options = new RewriteOptions()
        .AddRewrite("^/mynd/reception/newui/.*", "/mynd/reception/newui/", skipRemainingRules: true)
        .AddRewrite("^/clinic/newui/.*", "/clinic/newui/", skipRemainingRules: true);
    app.UseRewriter(options);
    */

    if (app.Environment.IsDevelopment()) // configure for dev environment
    {
        // Use dev error screen with a more detailed message.
        app.UseStatusCodePagesWithReExecute("/Home/ErrorDev", "?statusCode={0}");
        // enable all routes listing 
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
    
    Log.Information("Hbk.Platform startup complete.");

    // Register background tasks.
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            // Register background tasks.
            var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            recurringJobs.AddOrUpdate<ICentralScrutinizerService>("prune-users", css => css.PruneActiveUsers(), "1 1 * * *"); // every night at 1am
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "job init failed");
            throw;
        }
    }


    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
