using MassTransit;
using MediatR;
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
