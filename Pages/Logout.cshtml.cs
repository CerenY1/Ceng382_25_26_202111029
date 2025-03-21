using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace RazorPage.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Oturumu temizle
            HttpContext.Session.Clear();

            // Tüm çerezleri temizle
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            // Login sayfasına yönlendir
            return RedirectToPage("/Login");
        }
    }
}
