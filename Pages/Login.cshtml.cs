using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RazorPage.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public LoginModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var token = HttpContext.Session.GetString("Token");
            var sessionId = HttpContext.Session.GetString("SessionId");

            if (!string.IsNullOrEmpty(username) && 
                !string.IsNullOrEmpty(token) && 
                !string.IsNullOrEmpty(sessionId))
            {
                return RedirectToPage("/Index");
            }

            return Page(); 
        }

        public IActionResult OnPost()
        {
            var usersJsonPath = Path.Combine(_env.WebRootPath, "data", "users.json");
            var usersJson = System.IO.File.ReadAllText(usersJsonPath);
            var users = JsonSerializer.Deserialize<List<User>>(usersJson);

            var user = users?.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.IsActive);

            if (user == null)
            {
                ErrorMessage = "Username or password is incorrect.";
                return Page();
            }

            var token = Guid.NewGuid().ToString();
            var sessionId = HttpContext.Session.Id;

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("Token", token);
            HttpContext.Session.SetString("SessionId", sessionId);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(20),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("Username", user.Username, cookieOptions);
            Response.Cookies.Append("Token", token, cookieOptions);
            Response.Cookies.Append("SessionId", sessionId, cookieOptions);

            return RedirectToPage("/Index");
        }
    }
}