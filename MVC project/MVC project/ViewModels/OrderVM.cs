using MVC_Project.Models;

namespace MVC_Project.ViewModels
{
    public class OrderVM
    {
        public string Adress { get; set; }
        public List<BasketItem>? BasketItems { get; set; }
    }
}
