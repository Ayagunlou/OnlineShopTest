using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class ProductTypes
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Product Type")]
        public string ProductType { get; set; }
    }
}