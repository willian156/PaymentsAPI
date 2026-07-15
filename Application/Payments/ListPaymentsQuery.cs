using MassTransit;
using MediatR;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Application.Payments;

public class ListPaymentsQuery : IRequest<IReadOnlyList<PaymentDto>>
{
}
