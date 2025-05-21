using Flunt.Notifications;

namespace Order.Aplicacao.Respostas;

public class Resposta
{
    public Resposta()
    {
        Sucesso = true;
        Errors = null;
    }

    public Resposta(IEnumerable<Notification> errors)
    {
        Sucesso = false;
        Errors = errors;
    }

    public bool Sucesso { get; }

    public IEnumerable<Notification> Errors { get; }
}

public class Resposta<TRetorno> : Resposta
{
    public Resposta(TRetorno retorno) : base()
    {
        Retorno = retorno;
        Paginacao = null;
    }

    public Resposta(TRetorno retorno, Paginacao paginacao) : this(retorno)
    {
        Paginacao = paginacao;
    }

    public Resposta(IEnumerable<Notification> errors) : base(errors) { }

    public TRetorno Retorno { get; }

    public Paginacao Paginacao { get; }
}