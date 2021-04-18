using System;

namespace GloboTicket.Messages
{
    public class PaymentRequestMessage
    {
        public Guid BasketId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
