using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;
using System.Linq;
using System.Collections.Generic;

namespace RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        [BindProperty]
        public int? EditId { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (EditId.HasValue)
            {
                var existingClass = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
                if (existingClass != null)
                { 

                    
                    existingClass.ClassName = NewClass.ClassName;
                    existingClass.StudentCount = NewClass.StudentCount;
                    existingClass.Description = NewClass.Description;

                    
                    existingClass.EditID ??= $"CLS-{Guid.NewGuid().ToString().Substring(0, 8)}";

                    
                }
                EditId = null;
            }
            else
            {
                NewClass.Id = ClassList.Count > 0 ? ClassList.Max(c => c.Id) + 1 : 1;
                NewClass.EditID = $"CLS-{Guid.NewGuid().ToString().Substring(0, 8)}";

                ClassList.Add(NewClass);
            }

            // Formu temizle
            NewClass = new ClassInformationModel();
            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
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
                    EditID = classToEdit.EditID,
                    
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

            return RedirectToPage();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewClass.GenerateId();
            ClassList.Add(NewClass);

            return RedirectToPage();
        }
    }
}
