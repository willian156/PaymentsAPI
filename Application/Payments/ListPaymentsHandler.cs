using MassTransit;
using MediatR;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Application.Payments;

public class ListPaymentsHandler(IPaymentRepository r) : IRequestHandler<ListPaymentsQuery, IReadOnlyList<PaymentDto>>
{
    public async Task<IReadOnlyList<PaymentDto>> Handle(ListPaymentsQuery q, CancellationToken ct) => (await r.ListAsync(ct)).Select(Mapping.ToDto).ToList();
}
