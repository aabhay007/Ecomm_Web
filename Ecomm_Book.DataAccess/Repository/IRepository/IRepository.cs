using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Book.DataAccess.Repository.IRepository
{
    public interface IRepository<T>where T:class    //interface (for declaration)
    {
        void Add(T entity);  //generic method
        void Update(T entity);
        void Remove(T entity);
        void Remove(int id); //overloading
        void RemoveRange(IEnumerable<T> values);
        T Get(int id);
        IEnumerable<T> GetAll(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>,IOrderedQueryable<T>>orderBy=null,
        string includeProperties=null  //category,covertype
        );
        T FirstOrDefault(
            Expression<Func<T, bool>> filter=null,
            string includeProperties=null
            );  

   }
}
