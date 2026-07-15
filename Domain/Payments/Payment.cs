namespace PaymentsAPI.Domain.Payments;

public class Payment
{
    private Payment()
    {
    }

    public Payment(Guid orderId, Guid userId, Guid gameId, decimal amount)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        UserId = userId;
        GameId = gameId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? Reason { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }

    public void Process(bool approved)
    {
        if (Status != PaymentStatus.Pending)
            return;
        Status = approved ? PaymentStatus.Approved : PaymentStatus.Rejected;
        Reason = approved ? null : "Pagamento recusado pela simulação.";
        ProcessedAt = DateTimeOffset.UtcNow;
    }
}
