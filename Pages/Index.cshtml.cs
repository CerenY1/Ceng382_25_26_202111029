using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;
using Microsoft.AspNetCore.Http;
using RazorPage.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClassInformationTable NewClass { get; set; } = new ClassInformationTable();

        [BindProperty]
        public int? EditId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterByName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public List<ClassInformationTable> FilteredClasses { get; set; } = new();

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var token = HttpContext.Session.GetString("Token");
            var sessionId = HttpContext.Session.GetString("SessionId");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(sessionId))
            {
                var cookieUsername = Request.Cookies["Username"];
                var cookieToken = Request.Cookies["Token"];
                var cookieSessionId = Request.Cookies["SessionId"];

                if (!string.IsNullOrEmpty(cookieUsername) &&
                    !string.IsNullOrEmpty(cookieToken) &&
                    !string.IsNullOrEmpty(cookieSessionId))
                {
                    HttpContext.Session.SetString("Username", cookieUsername);
                    HttpContext.Session.SetString("Token", cookieToken);
                    HttpContext.Session.SetString("SessionId", cookieSessionId);

                    username = cookieUsername;
                    token = cookieToken;
                    sessionId = cookieSessionId;
                }
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(sessionId))
            {
                return RedirectToPage("/Login");
            }

            var query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(FilterByName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterByName));
            }

            int totalRecords = query.Count();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            FilteredClasses = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();



            return Page();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Classes.Add(NewClass);
            _context.SaveChanges();

            return RedirectToPage(new { FilterByName, PageNumber });
        }

        public IActionResult OnPostEdit()
        {
            if (!ModelState.IsValid)
                return Page();

            if (EditId.HasValue)
            {
                var existingClass = _context.Classes.Find(EditId.Value);
                if (existingClass != null)
                {
                    existingClass.ClassName = NewClass.ClassName;
                    existingClass.StudentCount = NewClass.StudentCount;
                    existingClass.Description = NewClass.Description;
                    _context.SaveChanges();
                }
                EditId = null;
            }

            return RedirectToPage(new { FilterByName, PageNumber });
        }

        public IActionResult OnPostStartEdit(int id)
        {
            var classToEdit = _context.Classes.Find(id);
            if (classToEdit != null)
            {
                NewClass = new ClassInformationTable
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    Description = classToEdit.Description
                };
                EditId = id;
            }

            ModelState.Clear();
            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = _context.Classes.Find(id);
            if (classToRemove != null)
            {
                classToRemove.IsActive = false; 
                _context.SaveChanges();
            }

            return RedirectToPage(new { FilterByName, PageNumber });
        }

        public IActionResult OnPostExport()
        {
            var username = HttpContext.Session.GetString("Username");
            var token = HttpContext.Session.GetString("Token");
            var sessionId = HttpContext.Session.GetString("SessionId");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(sessionId))
            {
                return RedirectToPage("/Login");
            }

            var query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(FilterByName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterByName));
            }

            var paged = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var selectedColumnsString = Request.Form["SelectedColumns"];
            var selectedColumns = selectedColumnsString.ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToList();

            var exportData = paged.Select(c =>
            {
                var result = new Dictionary<string, object>();

                if (selectedColumns.Count == 0)
                {
                    result["ClassName"] = c.ClassName;
                    result["StudentCount"] = c.StudentCount;
                    result["Description"] = c.Description;
                }
                else
                {
                    if (selectedColumns.Contains("Class Name")) result["ClassName"] = c.ClassName;
                    if (selectedColumns.Contains("Student Count")) result["StudentCount"] = c.StudentCount;
                    if (selectedColumns.Contains("Description")) result["Description"] = c.Description;
                }

                return result;
            }).ToList();

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
            var fileName = $"Export_Page_{PageNumber}_{DateTime.Now:yyyyMMddHHmmss}.json";
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(fileBytes, "application/json", fileName);
        }
    }
}
