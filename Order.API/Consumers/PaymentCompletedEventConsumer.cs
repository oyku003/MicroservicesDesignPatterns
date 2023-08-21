using MassTransit;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        public Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
