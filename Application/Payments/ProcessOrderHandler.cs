using MassTransit;
using MediatR;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Application.Payments;

public class ProcessOrderHandler(IPaymentRepository repo, IPublishEndpoint bus, IConfiguration config) : IRequestHandler<ProcessOrderCommand, PaymentDto>
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
            await bus.Publish(new PaymentProcessedEvent { OrderId = p.OrderId, UserId = p.UserId, GameId = p.GameId, Price = p.Amount, Status = p.Status == Domain.Payments.PaymentStatus.Approved ? Contracts.PaymentStatus.Approved : Contracts.PaymentStatus.Rejected, Reason = p.Reason, ProcessedAt = p.ProcessedAt!.Value, }, ct);
        }

        return Mapping.ToDto(p);
    }
}
