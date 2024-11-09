using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Book.Models
{
    public class MyOrderDetail
    {
        public int Id { get; set; }
        public int MyOrderHeaderId { get; set; }
        [ForeignKey(nameof(MyOrderHeaderId))]
        public MyOrderHeader MyOrderHeader { get; set; }
        public int MyProductId { get; set; }
        [ForeignKey(nameof(MyProductId))]
        public MyProduct MyProduct { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
