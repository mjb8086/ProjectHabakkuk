using HBKPlatform.Database;
using HBKPlatform.Repository;
using HBKPlatform.Repository.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HBKPlatform.Areas.Identity.Data;

//string webRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");

// BEGIN Builder.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
                builder.Configuration.GetConnectionString("ApplicationDbContext") ??
                throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")
            )
    );

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddScoped<IHbkContext, ApplicationDbContext>();
builder.Services.AddTransient<IPractitionerRepository, PractitionerRepository>();

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

    SeedData.Initialise(services);
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

