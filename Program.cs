using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using CorporateCMS.Data;
using CorporateCMS.Models; // added
using CorporateCMS.Middleware; // added

var builder = WebApplication.CreateBuilder(args);

// Kestrel sunucusunu yapılandır
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5084); // Tüm IP adreslerinden 5084 portunu dinle
});

// Add services to the container
// Kurumsal CMS - ASP.NET MVC Uygulaması
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity with Roles support (switch to ApplicationUser)
builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false; // Development için false
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddRoles<IdentityRole>() // Rol desteği ekle
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthSection["ClientId"] ?? string.Empty;
        options.ClientSecret = googleAuthSection["ClientSecret"] ?? string.Empty;
        options.CallbackPath = "/signin-google"; // Default callback path
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); 

// CORS policy if needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageContent", policy => policy.RequireRole("SuperAdmin", "Admin", "Editor"));
    options.AddPolicy("ViewAdmin", policy => policy.RequireRole("SuperAdmin", "Admin"));
});

var app = builder.Build();

// Initialize database and roles
await InitializeDatabase(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/500");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

// Development ortamında HTTPS yönlendirmesini devre dışı bırakma
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseGlobalExceptionHandler();

app.UseAuthentication(); // Authentication middleware
app.UseAuthorization();  // Authorization middleware

// Admin area route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

// Özel error sayfaları
app.MapControllerRoute(
    name: "404",
    pattern: "404",
    defaults: new { controller = "Error", action = "PageNotFound" });

app.MapControllerRoute(
    name: "500",
    pattern: "500",
    defaults: new { controller = "Error", action = "ServerError" });

// SEO-friendly URL için özel slug rotası (daha yüksek öncelik)
app.MapControllerRoute(
    name: "page",
    pattern: "{slug}",
    defaults: new { controller = "Page", action = "Details" },
    constraints: new { slug = @"^(?!Admin|Account|Error|Home|api|Identity).*$" }); // Admin, Account gibi routeleri hariç tut

// Standart controller rota sistemi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// Initialize database and create default roles/users
async Task InitializeDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Apply pending migrations (replaces EnsureCreated)
        await context.Database.MigrateAsync();
        
        // Create roles
        string[] roles = { "SuperAdmin", "Admin", "Editor", "Viewer" };
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        
        // Create default admin user
        var adminEmail = "admin@kurumsalcms.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                DisplayName = "Admin",
                LastLoginAt = DateTime.UtcNow
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}
