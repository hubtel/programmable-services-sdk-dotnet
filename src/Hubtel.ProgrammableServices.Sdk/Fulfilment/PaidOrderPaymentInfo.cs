using System;

namespace Hubtel.ProgrammableServices.Sdk.Fulfilment
{
    public class PaidOrderPaymentInfo
    {
        public string PaymentType { get; set; }
        public string PaymentDescription { get; set; }
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal AmountPaid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime PaymentDate { get; set; }
    }
}