using Order.Aplicacao.Dtos;
using Order.Aplicacao.Respostas;
using Order.Dominio.Entidades;
using MediatR;

namespace Order.Aplicacao.Queries;

public class PedidoQuery : Query<Pedido, object>
{
    /// <summary>
    /// Construtor
    /// </summary>
    private PedidoQuery() { }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="termo"></param>
    public PedidoQuery(string? termo)
    {
        Termo = termo;
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="termo"></param>
    /// <param name="paginacao"></param>
    public PedidoQuery(string? termo, Paginacao paginacao)
        : base(paginacao)
    {
        Termo = termo;
    }

    /// <summary>
    /// Termo
    /// </summary>
    public string? Termo { get; }

    /// <summary>
    /// Buscar dados
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="termo"></param>
    /// <param name="paginacao"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Resposta<IEnumerable<object>>> BuscarAsync(IMediator mediator, string? termo, PaginacaoRequest? paginacao, CancellationToken cancellationToken)
    {
        var query = new PedidoQuery(termo);
        return await mediator.Send(query, cancellationToken);
    }
}

