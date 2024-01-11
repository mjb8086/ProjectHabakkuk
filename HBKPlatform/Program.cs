using HBKPlatform.Areas.Account;
using HBKPlatform.Database;
using HBKPlatform.Database.Helpers;
using HBKPlatform.Repository;
using HBKPlatform.Repository.Implementation;
using HBKPlatform.Services;
using HBKPlatform.Services.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

const string hbkName =
    "    __  ______  __ __    ____  __      __  ____                   \n   / / / / __ )/ //_/   / __ \\/ /___ _/ /_/ __/___  _________ ___ \n  / /_/ / __  / ,<     / /_/ / / __ `/ __/ /_/ __ \\/ ___/ __ `__ \\\n / __  / /_/ / /| |   / ____/ / /_/ / /_/ __/ /_/ / /  / / / / / /\n/_/ /_/_____/_/ |_|  /_/   /_/\\__,_/\\__/_/  \\____/_/  /_/ /_/ /_/ \n";

// BEGIN Builder.
var builder = WebApplication.CreateBuilder(args);
var version = builder.Configuration.GetValue<double>("Version");

Console.WriteLine($"NowDoctor Ltd. Presents:\n{hbkName}\nVersion {version}. Starting up...");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
                builder.Configuration.GetConnectionString("HbkContext") ??
                throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")
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

builder.Services.AddTransient<IClinicService, ClinicService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IClientMessagingService, ClientMessagingService>();
builder.Services.AddTransient<ICacheService, CacheService>();
builder.Services.AddTransient<IClientRecordService, ClientRecordService>();
builder.Services.AddTransient<IAppointmentService, AppointmentService>();
builder.Services.AddTransient<IBookingService, BookingService>();

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    builder.WebHost.UseUrls("http://*:80", "https://*.443");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute( name: "MasterControlPanel", pattern: "{area:exists}/{controller=MCP}/{action=Index}/{id?}");
app.MapControllerRoute( name: "MyND", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
app.MapControllerRoute( name: "Client", pattern: "{area:exists}/{controller=Reception}/{action=Index}/{id?}");
app.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)).ToLower());
}

app.Run();
