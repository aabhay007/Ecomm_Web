using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Book.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<MyShoppingCart> ListCart { get; set; }
        public MyOrderHeader MyOrderHeader { get; set; }
        public IEnumerable<SelectListItem> AddressAnother { get; set; }
        public int SelectedAddressId { get; set; }

    }
}
