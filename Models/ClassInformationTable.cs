using System.ComponentModel.DataAnnotations;

namespace RazorPage.Models
{
    public class ClassInformationTable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        public int StudentCount { get; set; }

        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

    }
}
