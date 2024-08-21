using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AddToCart.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? username { get; set; }
        [Remote(action:"CheckExistingId", controller:"Auth")]
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? role { get; set; }
    }
}
