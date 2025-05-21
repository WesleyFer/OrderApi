using System.ComponentModel.DataAnnotations;

namespace Order.Aplicacao.Dtos;

public record ProdutoRequest(
    [Required(ErrorMessage = "Campo obrigatório")]
    string Nome,
    [Required(ErrorMessage = "Campo obrigatório")]
    int Quantidade,
    [Required(ErrorMessage = "Campo obrigatório")]
    decimal Preco);


public record AtualizarProdutoRequest(
    Guid Id,
    [Required(ErrorMessage = "Campo obrigatório")]
    string Nome,
    [Required(ErrorMessage = "Campo obrigatório")]
    int Quantidade,
    [Required(ErrorMessage = "Campo obrigatório")]
    decimal Preco);
