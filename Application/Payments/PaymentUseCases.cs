using MassTransit;
using MediatR;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Application.Payments;

public class ProcessOrderCommand : IRequest<PaymentDto>
{
    public OrderPlacedEvent Event { get; set; } = new();
}

public class GetPaymentQuery : IRequest<PaymentDto?>
{
    public Guid Id { get; set; }
}

public class ListPaymentsQuery : IRequest<IReadOnlyList<PaymentDto>> { }

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
}

public static class Mapping
{
    public static PaymentDto ToDto(Payment p) =>
        new()
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

public class ProcessOrderHandler(
    IPaymentRepository repo,
    IPublishEndpoint bus,
    IConfiguration config
) : IRequestHandler<ProcessOrderCommand, PaymentDto>
{
    public async Task<PaymentDto> Handle(ProcessOrderCommand c, CancellationToken ct)
    {
        var e = c.Event;
        var p = await repo.GetByOrderAsync(e.OrderId, ct);
        if (p is null)
        {
            p = new Payment(e.OrderId, e.UserId, e.GameId, e.Price);
            repo.Add(p);
            p.Process(config.GetValue("Payments:AlwaysApprove", true));
            await repo.SaveAsync(ct);
            await bus.Publish(
                new PaymentProcessedEvent
                {
                    OrderId = p.OrderId,
                    UserId = p.UserId,
                    GameId = p.GameId,
                    Price = p.Amount,
                    Status =
                        p.Status == Domain.Payments.PaymentStatus.Approved
                            ? Contracts.PaymentStatus.Approved
                            : Contracts.PaymentStatus.Rejected,
                    Reason = p.Reason,
                    ProcessedAt = p.ProcessedAt!.Value,
                },
                ct
            );
        }
        return Mapping.ToDto(p);
    }
}

public class GetPaymentHandler(IPaymentRepository r) : IRequestHandler<GetPaymentQuery, PaymentDto?>
{
    public async Task<PaymentDto?> Handle(GetPaymentQuery q, CancellationToken ct)
    {
        var p = await r.GetAsync(q.Id, ct);
        return p is null ? null : Mapping.ToDto(p);
    }
}

public class ListPaymentsHandler(IPaymentRepository r)
    : IRequestHandler<ListPaymentsQuery, IReadOnlyList<PaymentDto>>
{
    public async Task<IReadOnlyList<PaymentDto>> Handle(
        ListPaymentsQuery q,
        CancellationToken ct
    ) => (await r.ListAsync(ct)).Select(Mapping.ToDto).ToList();
}
