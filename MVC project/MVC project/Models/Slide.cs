using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Project.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Order { get; set; }
       


    }
}
