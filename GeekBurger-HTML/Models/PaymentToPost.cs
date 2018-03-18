using System;

namespace GeekBurger_HTML.Models
{
    public class PaymentToPost
    {
        public Guid OrderId { get; set; }
        public string CardNumber { get; set; }
        public string CardOwnerName { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string SecurityCode { get; set; }
        public Guid StoreId { get; set; }
    }
}