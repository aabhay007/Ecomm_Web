using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Book.Utility
{
    public static class SD  //standard dictionary
    {
        //covertype stored procedure
        public const string Proc_GetCoverTypes = "SP_Get_CoverTypes";
        public const string Proc_GetCoverType = "SP_Get_CoverType";
        public const string Proc_CreateCoverType = "SP_Create_CoverType";
        public const string Proc_UpdateCoverType = "SP_Update_CoverType";
        public const string Proc_DeleteCoverType = "SP_Delete_CoverType";


        //roles
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee User";
        public const string Role_Company = "Company User";
        public const string Role_Individual = "Individual User";

        //order status
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusProgress = "Progress";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefunded = "Refunded";

        //Payment Status
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayPayment = "PaymentStatusDelayed";
        public const string PaymentStatusRejected = "Rejected";

        //session
        public const string Ss_CartSessionCount = "Cart Count Session";

        public static double GetPriceBasedOnQuantity(double quantity,
            double price,double price50,double price100)
        {
            if (quantity < 50)
                return price;
            else if (quantity < 100)
                return price50;
            else return price100;
        }
    }
}
