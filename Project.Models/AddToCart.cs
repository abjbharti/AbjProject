using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class AddToCart
    {
        public int Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        public string ProductName { get; set; }

        [Required]
        public int ProductPrice { get; set; }

        [Required]
        public int ProductQuantity { get; set; }

        public Guid CustomerId { get; set; }

        public string CustomerEmail { get; set; }

        public bool IsCheckOut { get; set; } = true;
    }
}
