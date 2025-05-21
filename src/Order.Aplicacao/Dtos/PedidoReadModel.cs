using System.ComponentModel.DataAnnotations;

namespace Order.Aplicacao.Dtos;

public record PedidoReadModel(
    Guid Id,
    int CodigoPedido,
    int CodigoCliente,
    List<ProdutoReadModel> Itens);
