using HBKPlatform.Areas.Account;
using HBKPlatform.Database;
using HBKPlatform.Database.Helpers;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Repository;
using HBKPlatform.Repository.Implementation;
using HBKPlatform.Services;
using HBKPlatform.Services.Implementation;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// BEGIN Builder.
var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"NowDoctor Ltd. Presents:\n{Consts.HBK_NAME}\nVersion {Consts.VERSION}. Starting up...");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
                builder.Configuration.GetConnectionString("HbkContext") ??
                throw new InvalidOperationException("Connection string is invalid.")
            )
    );

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// To ensure custom claims (ClinicId, PracId, etc) are added to new identity when principal is refreshed.
builder.Services.ConfigureOptions<ConfigureSecurityStampOptions>();

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

builder.Services.AddTransient<IClinicService, ClinicService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IClientMessagingService, ClientMessagingService>();
builder.Services.AddTransient<ICacheService, CacheService>();
builder.Services.AddTransient<IClientRecordService, ClientRecordService>();
builder.Services.AddTransient<ITreatmentService, TreatmentService>();
builder.Services.AddTransient<IBookingService, BookingService>();
builder.Services.AddTransient<IConfigurationService, ConfigurationService>();
builder.Services.AddTransient<IDateTimeWrapper, DateTimeWrapper>();
builder.Services.AddTransient<IClientDetailsService, ClientDetailsService>();
builder.Services.AddTransient<IAvailabilityManagementService, AvailabilityManagementService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add services to the container.
builder.Services.AddControllersWithViews();

var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
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

app.UseAuthorization();

app.MapControllerRoute( name: "MCP", pattern: "{area:exists}/{controller=MCP}/{action=Index}/{id?}");
app.MapControllerRoute( name: "MyND", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
app.MapControllerRoute( name: "Client", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
app.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) // configure production
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    builder.WebHost.UseUrls("http://*:80", "https://*.443");
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
}
else // configure for dev environment
{
    app.UseStatusCodePagesWithReExecute("/Home/ErrorDev", "?statusCode={0}");
    app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)).ToLower());
}

app.Run();
