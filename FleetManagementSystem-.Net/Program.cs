using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Stores;
using FleetManagementSystem_.Net.Data;
using FleetManagementSystem_.Net.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<FMSUser,FMSRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddUserStore<FMSUserStore>()
    .AddRoleStore<FMSRoleStore>()
    .AddRoleManager<RoleManager<FMSRole>>()
    .AddUserManager<UserManager<FMSUser>>()
    .AddSignInManager<SignInManager<FMSUser>>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, AuthorisationHandler>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Login";
    options.LogoutPath = "/Identity/Logout";

    // Set the path the middleware will redirect to on 403 (Access Denied)
    options.AccessDeniedPath = "/Home/AccessDenied";

    // For AJAX/API calls, return 403 instead of redirecting
    options.Events.OnRedirectToAccessDenied = context =>
    {
        var isAjax = string.Equals(context.Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        var isApi = context.Request.Path.StartsWithSegments("/api");
        if (isAjax || isApi)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});
    

builder.Services.AddScoped<IUserStore<FMSUser>, FMSUserStore>();
builder.Services.AddScoped<IRoleStore<FMSRole>, FMSRoleStore>();
builder.Services.AddScoped<IStorageSiteRepository, StorageSiteRepository>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseSQLExceptionHandlerMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Area route must be registered before the default route so area controllers are found.
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.UseSession();

app.Run();
