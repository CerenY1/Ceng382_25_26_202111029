namespace RazorPage.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        public string? ClassName { get; set; }

        public int StudentCount { get; set; }

        public string? Description { get; set; }

        public string? EditID { get; set; }

        public ClassInformationModel()
        {
        }

        public void GenerateId()
        {
             if (string.IsNullOrEmpty(EditID)) 
            {
                EditID = $"CLS-{Guid.NewGuid().ToString().Substring(0, 8)}";
            }
        }
    }
}