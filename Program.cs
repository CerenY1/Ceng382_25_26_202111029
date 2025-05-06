using System.Text.Json;
using RazorPage.Models;
using System.IO;
using RazorPage.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SchoolDbContext>();
    context.Database.Migrate(); 
    DbInitializer.Seed(context);
}


app.UseHttpsRedirection(); 
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); 
app.UseAuthorization(); 
app.MapRazorPages();


app.MapGet("/", async context =>
{
    var session = context.Session;

    if (session.GetString("Username") != null &&
        session.GetString("Token") != null &&
        session.GetString("SessionId") != null)
    {
        context.Response.Redirect("/Index"); 
    }
    else
    {
        context.Response.Redirect("/Login"); 
    }
});
app.Run();
