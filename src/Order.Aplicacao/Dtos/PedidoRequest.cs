using System.ComponentModel.DataAnnotations;

namespace Order.Aplicacao.Dtos;

public record PedidoRequest(
    [Required(ErrorMessage = "Campo obrigatório")]
    int CodigoPedido,
    [Required(ErrorMessage = "Campo obrigatório")]
    int CodigoCliente,
    [Required(ErrorMessage = "Campo obrigatório")]
    List<ProdutoRequest> Itens);

public record AtualizarPedidoRequest(
    [Required(ErrorMessage = "Campo obrigatório")]
    int CodigoPedido,
    [Required(ErrorMessage = "Campo obrigatório")]
    int CodigoCliente,
    [Required(ErrorMessage = "Campo obrigatório")]
    List<AtualizarProdutoRequest> Itens);
