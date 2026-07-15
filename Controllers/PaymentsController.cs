using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentsAPI.Application.Payments;

namespace PaymentsAPI.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await sender.Send(new ListPaymentsQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var payment = await sender.Send(new GetPaymentQuery { Id = id }, ct);
        return payment is null ? NotFound() : Ok(payment);
    }
}
