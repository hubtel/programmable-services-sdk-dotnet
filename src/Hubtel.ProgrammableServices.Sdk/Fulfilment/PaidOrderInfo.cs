using System;
using System.Collections.Generic;

namespace Hubtel.ProgrammableServices.Sdk.Fulfilment
{
    public class PaidOrderInfo
    {
        public PaidOrderInfo()
        {
            Items = new List<PaidOrderInfoItem>();
            Payment = new PaidOrderPaymentInfo();
        }
        /// <summary>
        /// 
        /// </summary>
        public string CustomerMobileNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public string BranchName { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurringInvoiceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime OrderDate { get; set; }

        public List<PaidOrderInfoItem> Items { get; set; }
        public PaidOrderPaymentInfo Payment { get; set; }
    }
}