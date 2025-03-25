namespace RazorPage.Models
{
    public class ClassInformationModel
    {
        private static int _idSayac = 1; 

        public int Id { get; set; }

        public string? ClassName { get; set; }

        public int StudentCount { get; set; }

        public string ?Description { get; set; }

        public ClassInformationModel()
        {
            Id = _idSayac++;
        }
    }
}
