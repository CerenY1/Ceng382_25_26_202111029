using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;
using System.Linq;
using System.Collections.Generic;

namespace RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        // Statik listeyi burada tanımlıyoruz, tüm veriler bu listede tutulacak
        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        // Yeni sınıf verisini tutmak için
        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        // Düzenlenecek sınıfın id'si
        [BindProperty]
        public int? EditId { get; set; }

        public void OnGet()
        {
            // Sayfa ilk yüklendiğinde yapılacak işlemler
            // Veriler sayfa yüklendiğinde otomatik olarak ClassList'te olmalıdır.
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Geçersiz form verisi varsa sayfayı yeniden yükle
            }

            // Eğer EditId varsa, mevcut sınıfı düzenle
            if (EditId.HasValue)
            {
                var existingClass = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
                if (existingClass != null)
                {
                    // Mevcut sınıfı güncelle
                    existingClass.ClassName = NewClass.ClassName;
                    existingClass.StudentCount = NewClass.StudentCount;
                    existingClass.Description = NewClass.Description;
                }
            }
            else
            {
                // Yeni sınıf ekle
                ClassList.Add(new ClassInformationModel
                {
                    ClassName = NewClass.ClassName,
                    StudentCount = NewClass.StudentCount,
                    Description = NewClass.Description
                });
            }

            EditId = null; // Düzenleme işleminden sonra EditId'yi sıfırla
            return RedirectToPage();  // Sayfayı yeniden yükleyerek tabloyu güncelle
        }

        // Düzenleme işlemi için çağrılan metod
        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = IndexModel.ClassList.FirstOrDefault(c => c.Id == id); // Burada IndexModel.ClassList kullanılmalı
            if (classToEdit != null)
            {
                // Düzenlenecek sınıfın bilgilerini forma yerleştir
                NewClass = new ClassInformationModel
                {
                    // Id'yi doğrudan set etmiyoruz, sadece diğer alanları set ediyoruz.
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    Description = classToEdit.Description
                };
                EditId = id; // Düzenlenecek sınıfın id'sini sakla
            }

            return Page(); // Sayfayı güncelleyerek formu doldur
        }


        // Silme işlemi
        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                // Sınıfı listeden çıkar
                ClassList.Remove(classToRemove);
            }

            return RedirectToPage(); // Silme işleminden sonra sayfayı yeniden yükleyerek tabloyu güncelle
        }
    }
}
