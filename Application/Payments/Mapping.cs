using MassTransit;
using MediatR;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Application.Payments;

public static class Mapping
{
    public static PaymentDto ToDto(Payment p) => new()
    {
        Id = p.Id,
        OrderId = p.OrderId,
        UserId = p.UserId,
        GameId = p.GameId,
        Amount = p.Amount,
        Status = p.Status.ToString(),
        Reason = p.Reason,
        CreatedAt = p.CreatedAt,
        ProcessedAt = p.ProcessedAt,
    };
}
