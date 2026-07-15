using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentsAPI.Application.Payments;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Infrastructure;

public class PaymentRepository(PaymentsDbContext db) : IPaymentRepository
{
    public Task<Payment?> GetByOrderAsync(Guid id, CancellationToken ct) => db.Payments.FirstOrDefaultAsync(x => x.OrderId == id, ct);
    public Task<Payment?> GetAsync(Guid id, CancellationToken ct) => db.Payments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IReadOnlyList<Payment>> ListAsync(CancellationToken ct) => await db.Payments.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    public void Add(Payment p) => db.Payments.Add(p);
    public async Task SaveAsync(CancellationToken ct) => await db.SaveChangesAsync(ct);
}
