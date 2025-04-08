using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = GenerateFakeData();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        [BindProperty]
        public int? EditId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterByName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public List<ClassInformationTable> FilteredClasses { get; set; } = new();

        public void OnGet()
        {
            var query = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FilterByName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterByName, StringComparison.OrdinalIgnoreCase));
            }

            int totalRecords = query.Count();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            FilteredClasses = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            NewClass.Id = ClassList.Count > 0 ? ClassList.Max(c => c.Id) + 1 : 1;
            NewClass.EditID = $"CLS-{Guid.NewGuid().ToString().Substring(0, 8)}";
            ClassList.Add(NewClass);

            NewClass = new ClassInformationModel(); // formu sıfırla
            return RedirectToPage(new { FilterByName, PageNumber });
        }

        public IActionResult OnPostEdit()
        {
            if (!ModelState.IsValid)
                return Page();

            if (EditId.HasValue)
            {
                var existingClass = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
                if (existingClass != null)
                {
                    existingClass.ClassName = NewClass.ClassName;
                    existingClass.StudentCount = NewClass.StudentCount;
                    existingClass.Description = NewClass.Description;
                }
                EditId = null;
            }

            NewClass = new ClassInformationModel(); // formu sıfırla
            return RedirectToPage(new { FilterByName, PageNumber });
        }

        public IActionResult OnPostStartEdit(int id)
        {
            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                NewClass = new ClassInformationModel
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    Description = classToEdit.Description,
                    EditID = classToEdit.EditID
                };
                EditId = id;
            }

            ModelState.Clear();
            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                ClassList.Remove(classToRemove);
            }

            return RedirectToPage(new { FilterByName, PageNumber });
        }

        private static List<ClassInformationModel> GenerateFakeData()
        {
            var list = new List<ClassInformationModel>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"Class {i}",
                    Description = $"Sample description for Class {i}",
                    StudentCount = 20 + (i % 10),
                    EditID = $"CLS-{Guid.NewGuid().ToString().Substring(0, 8)}"
                });
            }
            return list;
        }
    }
}
