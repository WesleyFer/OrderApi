using Order.Dominio.Contratos;
using Order.Dominio.Enums;
using Flunt.Validations;

namespace Order.Dominio.Entidades;

public class Pedido : Entidade, IAggregateRoot
{
    private readonly List<Produto> _produtos;

    /// <summary>
    /// Construtor EF
    /// </summary>
    protected Pedido() { }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="codigoPedido"></param>
    /// <param name="codigoCliente"></param>
    public Pedido(int codigoPedido, int codigoCliente)
    {

        AddNotifications(new Contract<Pedido>()
            .Requires()
            .IsGreaterThan(codigoPedido, 0, "Pedido.CodigoPedido", "Código do pedido deve ser maior que zero.")
            .IsGreaterThan(codigoCliente, 0, "Pedido.CodigoCliente", "Código do cliente deve ser maior que zero.")
        );

        if (IsValid)
        {
            CodigoPedido = codigoPedido;
            CodigoCliente = codigoCliente;
            StatusPedido = EStatusPedido.Pendente;
            _produtos = new List<Produto>();
        }
    }

    /// <summary>
    /// Código Pedido
    /// </summary>
    public int CodigoPedido { get; private set; }

    /// <summary>
    /// Código Cliente
    /// </summary>
    public int CodigoCliente { get; private set; }

    /// <summary>
    /// Status do pedido
    /// </summary>
    public EStatusPedido StatusPedido { get; private set; }

    /// <summary>
    /// Produtos
    /// </summary>
    public IReadOnlyCollection<Produto> Produtos => _produtos.AsReadOnly();

    /// <summary>
    /// Adiciona um produto ao pedido
    /// </summary>
    /// <param name="produto"></param>
    public void AdicionarProduto(Produto produto)
    {
        AddNotifications(new Contract<Pedido>()
           .Requires()
           .IsNotNull(produto, "Pedido.Produto", "O produto não pode ser nulo.")
       );

        if (IsValid)
            _produtos.Add(produto);
    }

    /// <summary>
    /// Remove um produto do pedido
    /// </summary>
    /// <param name="produto"></param>
    public void RemoverProduto(Produto produto)
    {
        AddNotifications(new Contract<Pedido>()
            .Requires()
            .IsNotNull(produto, "Pedido.Produto", "O produto não pode ser nulo.")
        );

        if (IsValid)
            _produtos.Remove(produto);
    }

    /// <summary>
    /// Atualizar produtos
    /// </summary>
    /// <param name="novosProdutos"></param>
    public void AtualizarProdutos(List<Produto> novosProdutos)
    {
        foreach (var novoProduto in novosProdutos)
        {
            var produtoExistente = _produtos.FirstOrDefault(p => p.Id == novoProduto.Id);

            if (produtoExistente != null)
            {
                produtoExistente.AlterarNome(novoProduto.Nome);
                produtoExistente.AlterarQuantidade(novoProduto.Quantidade);
                produtoExistente.AlterarPreco(novoProduto.Preco);
            }
        }
    }
}
