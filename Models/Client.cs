using BlazorAppEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAppEcommerce.Models
{
    public class Client
    {
        [Key]
        public int CId { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        [ForeignKey("user")]
        public int UserId { get; set; }
        public User User { get; set; } // One-to-One relationship with User

    }
}
