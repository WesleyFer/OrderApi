using Order.Aplicacao.Comandos.Pedido;
using Order.Aplicacao.Dtos;
using Flunt.Notifications;
using Flunt.Validations;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Order.Aplicacao.Consumers;

public class PedidosConsumer
    : IConsumer<PedidoRequest>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PedidosConsumer> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="mediator"></param>
    public PedidosConsumer(
        IMediator mediator,
        ILogger<PedidosConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Consumer Pedido
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<PedidoRequest> context)
    {
        var request = context.Message;

        try
        {
            var contract = new Contract<Notification>()
               .Requires()
               .IsGreaterThan(request.CodigoPedido, 0, "CodigoPedido", "O código do pedido deve ser maior que zero.")
               .IsGreaterThan(request.CodigoCliente, 0, "CodigoCliente", "O código do cliente deve ser maior que zero.")
               .IsGreaterThan(request.Itens.Count, 0, "Produtos", "O pedido deve conter pelo menos um produto.");

            if (!contract.IsValid)
            {
                foreach (var notification in contract.Notifications)
                {
                    _logger.LogError($"Validação falhou: {notification.Message}");
                }

                throw new Exception("Falha na validação dos dados do pedido.");
            }

            _logger.LogInformation($"[Pedido recebido] Código: {request.CodigoPedido}, Cliente: {request.CodigoCliente}, Itens: {request.Itens.Count}");
            await CriarPedidoComando.CriarComandoAsync(_mediator, request, context.CancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[Erro crítico] Falha ao processar o pedido {request.CodigoPedido}");
            throw;
        }
    }
}
