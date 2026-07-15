namespace PaymentsAPI.Domain.Payments;

public interface IPaymentRepository
{
    Task<Payment?> GetByOrderAsync(Guid id, CancellationToken ct);
    Task<Payment?> GetAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Payment>> ListAsync(CancellationToken ct);
    void Add(Payment p);
    Task SaveAsync(CancellationToken ct);
}
