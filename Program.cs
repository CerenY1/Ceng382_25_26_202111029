using System.Text.Json;
using RazorPage.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection(); // Eksikse cookie çalışmayabilir!
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // DİKKAT: Routing'den sonra çağrılmalı
app.UseAuthorization(); 
app.MapRazorPages();


app.MapGet("/", async context =>
{
    var session = context.Session;

    if (session.GetString("Username") != null &&
        session.GetString("Token") != null &&
        session.GetString("SessionId") != null)
    {
        context.Response.Redirect("/Index"); // Giriş yapmışsa Index'e
    }
    else
    {
        context.Response.Redirect("/Login"); // Giriş yapmamışsa Login'e
    }
});
app.Run();
