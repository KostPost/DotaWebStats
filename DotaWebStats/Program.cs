using DotaWebStats.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<SteamAuthService>();
builder.Services.AddScoped<ISteamAuthService, SteamAuthService>();

builder.Services.AddScoped<DotaDataService>();
builder.Services.AddScoped<IDotaDataService, DotaDataService>();

var app = builder.Build();

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