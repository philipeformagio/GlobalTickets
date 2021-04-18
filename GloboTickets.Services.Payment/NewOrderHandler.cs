using GloboTicket.Messages;
using Rebus.Handlers;
using System;
using System.Threading.Tasks;

namespace GloboTickets.Services.Payment
{
    public class NewOrderHandler : IHandleMessages<PaymentRequestMessage>
    {
        public Task Handle(PaymentRequestMessage message)
        {
            // Ex.: Pay and send an email
            Console.WriteLine($"Payment request received for basket id {message.BasketId}, user id {message.UserId} " +
                $"and the total price of the transaction is {message.TotalPrice}.");
            return Task.CompletedTask;
        }
    }
}