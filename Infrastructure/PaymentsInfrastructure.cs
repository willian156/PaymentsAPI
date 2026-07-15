using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentsAPI.Application.Payments;
using PaymentsAPI.Contracts;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Infrastructure;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> o) : DbContext(o)
{
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Payment>(e =>
        {
            e.ToTable("payments");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.OrderId).IsUnique();
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.Amount).HasPrecision(18, 2);
        });
    }
}

public class PaymentRepository(PaymentsDbContext db) : IPaymentRepository
{
    public Task<Payment?> GetByOrderAsync(Guid id, CancellationToken ct) =>
        db.Payments.FirstOrDefaultAsync(x => x.OrderId == id, ct);

    public Task<Payment?> GetAsync(Guid id, CancellationToken ct) =>
        db.Payments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Payment>> ListAsync(CancellationToken ct) =>
        await db.Payments.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToListAsync(ct);

    public void Add(Payment p) => db.Payments.Add(p);

    public async Task SaveAsync(CancellationToken ct) => await db.SaveChangesAsync(ct);
}

public class OrderPlacedConsumer(ISender sender)
    : IConsumer<OrderPlacedEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedEvent> c) =>
        await sender.Send(new ProcessOrderCommand { Event = c.Message }, c.CancellationToken);
}
