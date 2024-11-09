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
    public class MyOrderHeaderRepository:Repository<MyOrderHeader>,IMyOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;
        public MyOrderHeaderRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
