using MassTransit;
using MediatR;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Application.Payments;

public class GetPaymentHandler(IPaymentRepository r) : IRequestHandler<GetPaymentQuery, PaymentDto?>
{
    public async Task<PaymentDto?> Handle(GetPaymentQuery q, CancellationToken ct)
    {
        var p = await r.GetAsync(q.Id, ct);
        return p is null ? null : Mapping.ToDto(p);
    }
}
