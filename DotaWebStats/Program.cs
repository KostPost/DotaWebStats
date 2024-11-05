using DotaWebStats.Models;
using DotaWebStats.Services;
using DotaWebStats.Services.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<SteamAuthService>();
builder.Services.AddScoped<ISteamAuthService, SteamAuthService>();

builder.Services.AddScoped<DotaDataService>();
builder.Services.AddScoped<IDotaDataService, DotaDataService>();

builder.Services.AddScoped<ApiService>();

builder.Services.AddScoped<DotaDataHelper>();
builder.Services.AddScoped<DotaHeroHelper>();
builder.Services.AddScoped<DotaItemHelper>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dotaHeroHelper = scope.ServiceProvider.GetRequiredService<DotaHeroHelper>();
    await DotaHeroHelper.InitializeAsync();
    
    var dotaItemHelper = scope.ServiceProvider.GetRequiredService<DotaItemHelper>();
    await DotaItemHelper.InitializeAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();