using Order.Aplicacao.Comandos.Pedido;
using Order.Aplicacao.Dtos;
using Order.Aplicacao.Queries;
using Order.Aplicacao.Respostas;
using Order.Dominio.Contratos;
using Order.Dominio.Entidades;
using MassTransit;
using MediatR;

namespace Order.Aplicacao.Handlers;

internal class PedidoHandler
    : IRequestHandler<PedidoQuery, Resposta<IEnumerable<object>>>,
    IRequestHandler<CriarPedidoComando, Resposta<Guid>>,
    IRequestHandler<AtualizarPedidoComando, Resposta<Guid>>,
    IRequestHandler<ExcluirPedidoComando, Resposta<Guid>>,
    IRequestHandler<EnviarPedidoComando, Resposta<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IQueryContexto _query;
    private readonly IPublishEndpoint _publishEndpoint;
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="uow"></param>
    /// <param name="query"></param>
    public PedidoHandler(IUnitOfWork uow, IQueryContexto query, IPublishEndpoint publishEndpoint)
    {
        _uow = uow;
        _query = query;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Resposta<IEnumerable<object>>> Handle(PedidoQuery request, CancellationToken cancellationToken)
    {
        if (!request.IsValid)
            return new Resposta<IEnumerable<object>>(request.Notifications);

        var query = _query.QueryPedidos;

        if (!string.IsNullOrEmpty(request.Termo))
            query = query.Where(c => c.CodigoCliente.ToString().ToLower().Contains(request.Termo.ToLower()) || c.CodigoPedido.ToString().ToLower().Contains(request.Termo.ToLower()) || c.Produtos.Any(p => p.Nome.ToLower().Contains(request.Termo.ToLower())));

        var resultado = query
            .Select(c => new PedidoReadModel(c.Id, c.CodigoPedido, c.CodigoCliente, c.Produtos.Select(p => new ProdutoReadModel(p.Id, p.Nome, p.Quantidade, p.Preco)).ToList()))
            .ToList()
            .OrderBy(c => c.CodigoPedido);

        if (!resultado.Any())
            return new Resposta<IEnumerable<object>>(new List<object>(), new Paginacao(request.Paginacao.Pagina, request.Paginacao.TotalPorPagina, 0));

        var total = resultado.Count();
        var pagina = request.Paginacao.Pagina;
        var totalPorPagina = request.Paginacao.TotalPorPagina;
        var resultadoParticionado = resultado.Skip((pagina - 1) * totalPorPagina).Take(totalPorPagina);

        return new Resposta<IEnumerable<object>>(resultadoParticionado, new Paginacao(pagina, totalPorPagina, total));
    }

    public async Task<Resposta<Guid>> Handle(CriarPedidoComando request, CancellationToken cancellationToken)
    {
        if (!request.IsValid)
            return new Resposta<Guid>(request.Notifications);

        var repositorio = _uow.Repositorio<Pedido>();

        var pedido = new Pedido(request.CodigoPedido, request.CodigoCliente);

        foreach (var produto in request.Produtos)
        {
            pedido.AdicionarProduto(produto);
        }

        await repositorio.InserirAsync(pedido, cancellationToken);

        if (await _uow.CommitAsync(cancellationToken) < 0)
        {
            request.AddNotification("", "Erro ao salvar no servidor");
            return new Resposta<Guid>(Guid.Empty);
        }

        return new Resposta<Guid>(pedido.Id);
    }

    public async Task<Resposta<Guid>> Handle(AtualizarPedidoComando request, CancellationToken cancellationToken)
    {
        if (!request.IsValid)
            return new Resposta<Guid>(request.Notifications);

        var repositorio = _uow.Repositorio<Pedido>();

        var resultado = await repositorio.BuscarAsync(c => c.Id == request.Id, cancellationToken, i => i.Produtos);
        var pedido = resultado.FirstOrDefault();

        if (pedido == null)
        {
            request.AddNotification("", "Não foi possível encontrar pedido especifico.");
            return new Resposta<Guid>(request.Notifications);
        }

        pedido.AtualizarProdutos(request.Produtos.ToList());

        await repositorio.EditarAsync(pedido, cancellationToken);

        if (await _uow.CommitAsync(cancellationToken) < 0)
        {
            request.AddNotification("", "Erro ao atualizar no servidor");
            return new Resposta<Guid>(Guid.Empty);
        }

        return new Resposta<Guid>(pedido.Id);
    }

    public async Task<Resposta<Guid>> Handle(ExcluirPedidoComando request, CancellationToken cancellationToken)
    {
        if (!request.IsValid)
            return new Resposta<Guid>(request.Notifications);

        var repositorio = _uow.Repositorio<Pedido>();

        var resultado = await repositorio.BuscarAsync(c => c.Id == request.Id, cancellationToken);
        var pedido = resultado.FirstOrDefault();

        if (pedido == null)
        {
            request.AddNotification("", "Não foi possível encontrar pedido especifico.");
            return new Resposta<Guid>(request.Notifications);
        }

        await repositorio.ExcluirAsync(pedido, cancellationToken);

        if (await _uow.CommitAsync(cancellationToken) < 0)
        {
            request.AddNotification("", "Erro ao deletar no servidor");
            return new Resposta<Guid>(Guid.Empty);
        }

        return new Resposta<Guid>(pedido.Id);
    }

    public async Task<Resposta<bool>> Handle(EnviarPedidoComando request, CancellationToken cancellationToken)
    {
        if (!request.IsValid)
            return new Resposta<bool>(request.Notifications);

        foreach (var pedido in request.Pedidos)
        {
            await _publishEndpoint.Publish(pedido, cancellationToken);
        }

        return new Resposta<bool>(true);
    }
}
