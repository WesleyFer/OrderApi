using Flunt.Validations;

namespace Order.Dominio.Entidades;

public class Produto : Entidade
{
    /// <summary>
    /// Construtor EF
    /// </summary>
    protected Produto() { }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="nome"></param>
    /// <param name="quantidade"></param>
    /// <param name="preco"></param>
    public Produto(string nome, int quantidade, decimal preco)
    {
        AddNotifications(new Contract<Produto>()
             .Requires()
             .IsNotNullOrEmpty(nome, "Produto.Nome", "O nome do produto é obrigatório.")
             .IsGreaterThan(quantidade, 0, "Produto.Quantidade", "A quantidade deve ser maior que zero.")
             .IsGreaterThan(preco, 0, "Produto.Preco", "O preço deve ser maior que zero.")
         );

        if (IsValid)
        {
            Nome = nome;
            Quantidade = quantidade;
            Preco = preco;
        }
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="id"></param>
    /// <param name="nome"></param>
    /// <param name="quantidade"></param>
    /// <param name="preco"></param>
    public Produto(Guid id, string nome, int quantidade, decimal preco)
        : base(id)
    {
        AddNotifications(new Contract<Produto>()
             .Requires()
             .IsNotNullOrEmpty(nome, "Produto.Nome", "O nome do produto é obrigatório.")
             .IsGreaterThan(quantidade, 0, "Produto.Quantidade", "A quantidade deve ser maior que zero.")
             .IsGreaterThan(preco, 0, "Produto.Preco", "O preço deve ser maior que zero.")
         );

        if (IsValid)
        {
            Nome = nome;
            Quantidade = quantidade;
            Preco = preco;
        }
    }

    /// <summary>
    /// Nome
    /// </summary>
    public string Nome { get; private set; }

    /// <summary>
    /// Quantidade
    /// </summary>
    public int Quantidade { get; private set; }

    /// <summary>
    /// Preço
    /// </summary>
    public decimal Preco { get; private set; }

    /// <summary>
    /// Atualiza o nome do produto
    /// </summary>
    public Produto AlterarNome(string novoNome)
    {
        AddNotifications(new Contract<Produto>()
            .Requires()
            .IsNotNullOrEmpty(novoNome, "Produto.Nome", "O nome do produto é obrigatório.")
        );

        if (IsValid)
            Nome = novoNome;

        return this;
    }

    /// <summary>
    /// Atualiza a quantidade do produto
    /// </summary>
    public Produto AlterarQuantidade(int novaQuantidade)
    {
        AddNotifications(new Contract<Produto>()
            .Requires()
            .IsGreaterThan(novaQuantidade, 0, "Produto.Quantidade", "A quantidade deve ser maior que zero.")
        );

        if (IsValid)
            Quantidade = novaQuantidade;

        return this;
    }

    /// <summary>
    /// Atualiza o preço do produto
    /// </summary>
    public Produto AlterarPreco(decimal novoPreco)
    {
        AddNotifications(new Contract<Produto>()
            .Requires()
            .IsGreaterThan(novoPreco, 0, "Produto.Preco", "O preço deve ser maior que zero.")
        );

        if (IsValid)
            Preco = novoPreco;

        return this;
    }

}
