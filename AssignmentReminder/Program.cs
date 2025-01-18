using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AssignmentReminder.Data;
using Hangfire;
using Hangfire.MemoryStorage;
using AssignmentReminder.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Configure email service
builder.Services.AddScoped<EmailService>();


// Configure application cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Events.OnRedirectToLogin = context =>
    {
        if (!string.IsNullOrEmpty(context.Request.Query["ReturnUrl"]))
        {
            context.Response.Redirect(context.Request.Query["ReturnUrl"]);
        }
        else
        {
            context.Response.Redirect("/Assignments"); // Default redirect
        }
        return Task.CompletedTask;
    };
});

// Add Hangfire services with in-memory storage
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Enable Hangfire Dashboard
app.UseHangfireDashboard();

// Schedule recurring jobs
RecurringJob.AddOrUpdate<EmailService>(
    "SendDueSoonReminders",
    service => service.SendDueSoonReminders(),
    Cron.Hourly // Adjust the schedule as needed
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
