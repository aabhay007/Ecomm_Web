using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Book.Models
{
    public class MyShoppingCart
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public int MyProductId { get; set; }
        [ForeignKey("MyProductId")]
        public MyProduct MyProduct { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
