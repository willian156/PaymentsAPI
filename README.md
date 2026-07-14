# PaymentsAPI

Microsserviço responsável por consumir `OrderPlacedEvent`, simular o processamento do pagamento e publicar `PaymentProcessedEvent` com status `Approved` ou `Rejected`.

## Executar

```bash
dotnet run --project PaymentsAPI.sln
```

## Variáveis de ambiente

- `ASPNETCORE_URLS`
- `RabbitMq__Host`
- `RabbitMq__Username`
- `RabbitMq__Password`
- `RabbitMq__OrderQueue`
- `Payments__AlwaysApprove`

## Docker

```bash
docker build -t fcg/payments-api:latest .
```

## Kubernetes

```bash
kubectl apply -f k8s/
```
