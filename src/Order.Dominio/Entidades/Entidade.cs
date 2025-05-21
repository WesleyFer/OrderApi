using Flunt.Notifications;

namespace Order.Dominio.Entidades;

public abstract class Entidade : Notifiable<Notification>
{
    protected Entidade(Guid? id = null)
    {
        Id = id.HasValue && id.Value != Guid.Empty ? id.Value : Guid.NewGuid();
    }

    public Guid Id { get; private set; }

    public DateTime DataCriacao { get; private set; } = DateTime.UtcNow;

    public DateTime DataAtualizacao { get; private set; } = DateTime.UtcNow;
}