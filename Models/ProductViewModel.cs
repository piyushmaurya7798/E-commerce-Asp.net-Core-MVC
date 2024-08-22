using System.ComponentModel.DataAnnotations;

namespace AddToCart.Models
{
    public class ProductViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public IFormFile Image { get; set; }
        public double Price { get; set; }
    }
}
