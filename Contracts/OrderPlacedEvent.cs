using MassTransit;

namespace PaymentsAPI.Contracts;

[MessageUrn("Fcg.Contracts:OrderPlacedEvent")]
[EntityName("fcg-order-placed")]
public class OrderPlacedEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset PlacedAt { get; set; }
}
