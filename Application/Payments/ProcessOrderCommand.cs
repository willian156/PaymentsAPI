using MassTransit;
using MediatR;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Application.Payments;

public class ProcessOrderCommand : IRequest<PaymentDto>
{
    public OrderPlacedEvent Event { get; set; } = new();
}
