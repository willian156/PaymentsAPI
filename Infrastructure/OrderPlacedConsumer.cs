using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentsAPI.Application.Payments;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Infrastructure;

public class OrderPlacedConsumer(ISender sender) : IConsumer<OrderPlacedEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedEvent> c) => await sender.Send(new ProcessOrderCommand { Event = c.Message }, c.CancellationToken);
}
