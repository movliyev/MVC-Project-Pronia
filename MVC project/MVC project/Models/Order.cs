namespace MVC_Project.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string  Adress { get; set; }
        public bool? Status { get; set; }
        public decimal TotalPrice { get; set; }

        public List<BasketItem> BasketItems { get; set; }
        public DateTime PurchasedAt { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}
