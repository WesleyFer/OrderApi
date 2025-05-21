using Order.Dominio.Entidades;

namespace Order.Dominio.Contratos;

public interface IUnitOfWork
{
    IRepositorio<TEntidade> Repositorio<TEntidade>() where TEntidade : Entidade, IAggregateRoot;

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork<TContexto> : IUnitOfWork where TContexto : IQueryContexto { }