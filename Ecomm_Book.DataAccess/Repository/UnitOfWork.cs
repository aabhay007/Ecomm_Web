using Ecomm_Book.DataAccess.Data;
using Ecomm_Book.DataAccess.Repository.IRepository;
using Ecomm_Book.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Book.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
            public UnitOfWork(ApplicationDbContext context)
        {
                _context= context;
            Category = new CategoryRepository(_context);
            CoverType= new CoverTypeRepository(_context);   
            SP_CALL= new SP_CAll(_context);
            MyProduct=new ProductRepository(_context);
            Company=new CompanyRepository(_context);
            ApplicationUser=new ApplicationUserRepository(_context);
            MyOrderDetail = new MyOrderDetailRepository(_context);
            MyOrderHeader = new MyOrderHeaderRepository(_context);
            MyShoppingCart = new MyShoppingCartRepository(_context);

        }
        public ICategoryRepository Category { set; get; }

        public ICoverTypeRepository CoverType { set; get; }
        public ISP_CALL SP_CALL { set; get; }
        public IProductRepository MyProduct { get; set; }
        public ICompanyRepository Company { get; set; }
        public IApplicationUserRepository ApplicationUser { get; set; }
        public IMyOrderDetailRepository MyOrderDetail { set; get; }
        public IMyOrderHeaderRepository MyOrderHeader { get; set; }
        public IMyShoppingCartRepository MyShoppingCart { set; get; }


        public void save()
        {
            _context.SaveChanges();
        }
    }
}
