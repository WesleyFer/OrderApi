using Order.Dominio.Entidades;

namespace Order.Dominio.Contratos;

public interface IQueryContexto
{
    /// <summary>
    /// Query Pedidos
    /// </summary>
    IQueryable<Pedido> QueryPedidos { get; }

    /// <summary>
    /// Query Produtos
    /// </summary>
    IQueryable<Produto> QueryProdutos { get; }
}
