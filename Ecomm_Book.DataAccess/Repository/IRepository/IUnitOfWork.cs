using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Book.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        ISP_CALL SP_CALL { get; }
        IProductRepository MyProduct {get;}
        ICompanyRepository Company { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IMyOrderDetailRepository MyOrderDetail { get; }
        IMyOrderHeaderRepository MyOrderHeader { get; }
        IMyShoppingCartRepository MyShoppingCart { get; }
        void save();
    }
}
