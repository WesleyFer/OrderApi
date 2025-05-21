using Order.Aplicacao.Dtos;
using Order.Aplicacao.Respostas;
using Order.Dominio.Entidades;
using Flunt.Notifications;
using Flunt.Validations;
using MediatR;

namespace Order.Aplicacao.Comandos.Pedido;

public class EnviarPedidoComando : Comando<bool>
{

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="pedidos"></param>
    public EnviarPedidoComando(List<PedidoRequest> pedidos)
    {
        Pedidos = pedidos;

        foreach (var pedido in Pedidos)
        {
            AddNotifications(new Contract<Notification>()
               .Requires()
               .IsGreaterThan(pedido.CodigoPedido, 0, "CodigoPedido", "O código do pedido deve ser maior que zero.")
               .IsGreaterThan(pedido.CodigoCliente, 0, "CodigoCliente", "O código do cliente deve ser maior que zero.")
               .IsGreaterThan(pedido.Itens?.Count ?? 0, 0, "Itens", "O pedido deve conter pelo menos um produto.")
            );
        }
    }

    /// <summary>
    /// Construtor
    /// </summary>
    public List<PedidoRequest> Pedidos { get; set; }

    /// <summary>
    /// Criar Comando
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="pedidos"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Resposta<bool>> CriarComandoAsync(IMediator mediator, List<PedidoRequest> pedidos, CancellationToken cancellationToken)
    {
        var comando = new EnviarPedidoComando(pedidos);
        return await mediator.Send(comando, cancellationToken);
    }
}
