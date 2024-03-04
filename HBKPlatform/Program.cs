using HBKPlatform.Areas.Account;
using HBKPlatform.Database;
using HBKPlatform.Database.Helpers;
using HBKPlatform.Extensions;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Middleware;
using HBKPlatform.Repository;
using HBKPlatform.Repository.Implementation;
using HBKPlatform.Services;
using HBKPlatform.Services.Implementation;
using Karambolo.Extensions.Logging.File;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

// BEGIN Builder.
var builder = WebApplication.CreateBuilder(args);

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

// To ensure custom claims (ClinicId, PracId, etc) are added to new identity when principal is refreshed.
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

builder.Services.AddScoped<TenancyMiddleware>();
builder.Services.AddScoped<CentralScrutinizerMiddleware>();

// Also use scoped for the cache and user service - both are utilised regularly
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

// Transient - created each time it is required. Use on web services, because they will typically serve one action
// to the controller.
builder.Services.AddTransient<IClinicService, ClinicService>();
builder.Services.AddTransient<IClientMessagingService, ClientMessagingService>();
builder.Services.AddTransient<IClientRecordService, ClientRecordService>();
builder.Services.AddTransient<ITreatmentService, TreatmentService>();
builder.Services.AddTransient<IBookingService, BookingService>();
builder.Services.AddTransient<IConfigurationService, ConfigurationService>();
builder.Services.AddTransient<IClientDetailsService, ClientDetailsService>();
builder.Services.AddTransient<IAvailabilityManagementService, AvailabilityManagementService>();
builder.Services.AddTransient<IMcpService, McpService>();

// Singleton - created once at startup. Use only where immutability or heftiness is likely. i.e. a distributed cache.
builder.Services.AddSingleton<IDateTimeWrapper, DateTimeWrapper>();
builder.Services.AddSingleton<ICentralScrutinizerService, CentralScrutinizerService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
            builder.Configuration.GetConnectionString("HbkContext") ??
            throw new InvalidOperationException("Connection string is invalid.")
        )
    );

// Logging - mostly configured in appsettings.json
builder.Services.AddLogging(lb =>
{
    lb.AddConfiguration(builder.Configuration.GetSection("Logging"));
    lb.AddFile(o => o.RootPath = builder.Environment.ContentRootPath);
    lb.AddFile<InfoFileLoggerProvider>(configure: o => o.RootPath = builder.Environment.ContentRootPath);
});

// Routing config - enable lowercase URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add services to the container.
builder.Services.AddControllersWithViews();

var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
    
    // Enable Npgsql native logging to console so we can actually see parameters.
    var loggerFactory = LoggerFactory.Create(lbuilder =>
    {
        lbuilder.AddConsole();
        lbuilder.AddConfiguration(builder.Configuration.GetRequiredSection("Logging"));
    });
    NpgsqlLoggingConfiguration.InitializeLogging(loggerFactory, parameterLoggingEnabled: true);
}

// END builder, create the webapp instance...
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await SeedNFeed.Initialise(services, new PasswordHasher<User>());
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Register middleware
app.UseMiddleware<TenancyMiddleware>();
app.UseMiddleware<CentralScrutinizerMiddleware>();

app.UseAuthorization();

app.MapControllerRoute( name: "MCP", pattern: "{area:exists}/{controller=MCP}/{action=Index}/{id?}");
app.MapControllerRoute( name: "MyND", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
app.MapControllerRoute( name: "Client", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
app.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // configure for dev environment, enable all routes listing 
{
    app.UseStatusCodePagesWithReExecute("/Home/ErrorDev", "?statusCode={0}");
    app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)).ToLower());
    
}
else // configure production
{
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    builder.WebHost.UseUrls("http://*:80", "https://*.443");
}

await using (ServiceProvider sp = builder.Services.BuildServiceProvider())
{
    // create logger
    ILogger<Program> logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
    logger.LogInformation("HBKPlatform startup complete.");
}
app.Run();
