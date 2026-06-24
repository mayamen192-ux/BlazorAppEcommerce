using BlazorAppEcommerce.Models;
using System.ComponentModel.DataAnnotations;

namespace BlazorAppEcommerce.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(12, ErrorMessage = "Category name cannot exceed 12 characters.")]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; } // One-to-Many relationship
    }
}
