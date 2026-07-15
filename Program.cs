using PaymentsAPI.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPaymentsApi(builder.Configuration);

var app = builder.Build();

await app.InitializePaymentsDatabaseAsync();
app.UsePaymentsApi();

app.Run();
