using Order.Aplicacao.Contratos;
using Order.Aplicacao.Respostas;
using Flunt.Notifications;

namespace Order.Aplicacao.Queries;

public abstract class Query<TResultado> : Notifiable<Notification>, IQuery<TResultado>
{
    protected Query() : this(new Paginacao(1, 10, 0)) { }
    protected Query(Paginacao paginacao) : this(paginacao, Array.Empty<string>()) { }
    protected Query(Paginacao paginacao, IEnumerable<string> campos) : this(paginacao, string.Empty, campos) { }
    protected Query(Paginacao paginacao, string ordenacao, IEnumerable<string> campos) : base()
    {
        Paginacao = paginacao;
        Campos = campos;
        Ordenacao = ordenacao;
    }

    public Paginacao Paginacao { get; protected set; }
    public string Ordenacao { get; protected set; }
    public IEnumerable<string> Campos { get; protected set; }
}

public abstract class Query<TEntidade, TResultado> :
      Query<TResultado>,
      IQuery<TEntidade, TResultado>
{
    protected Query() : base() { }
    protected Query(Paginacao paginacao) : base(paginacao) { }
    protected Query(Paginacao paginacao, IEnumerable<string> campos) : base(paginacao, campos) { }
    protected Query(Paginacao paginacao, string ordenacao, IEnumerable<string> campos) : base(paginacao, ordenacao, campos) { }
}