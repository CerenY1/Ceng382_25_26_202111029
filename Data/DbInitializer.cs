using RazorPage.Models;

namespace RazorPage.Data
{
    public static class DbInitializer
    {
        public static void Seed(SchoolDbContext context)
        {
            if (context.Classes.Any())
                return; 

            var classes = new List<ClassInformationTable>();
            for (int i = 1; i <= 100; i++)
            {
                classes.Add(new ClassInformationTable
                {
                    ClassName = $"Class {i}",
                    StudentCount = 20 + (i % 10),
                    Description = $"This is the description for Class {i}",
                    IsActive = true
                });

            }
           
            context.Classes.AddRange(classes);
            context.SaveChanges();
        }
    }
}
