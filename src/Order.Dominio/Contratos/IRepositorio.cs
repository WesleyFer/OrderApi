using System.Linq.Expressions;

namespace Order.Dominio.Contratos;

public interface IRepositorio<TEntidade> : IDisposable where TEntidade : class, IAggregateRoot
{
    Task<IEnumerable<TEntidade>> BuscarAsync(Expression<Func<TEntidade, bool>>? filtro = null, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TEntidade>> BuscarAsync(Expression<Func<TEntidade, bool>>? filtro = null, CancellationToken cancellationToken = default, params Expression<Func<TEntidade, object>>[] includes);

    Task InserirAsync(TEntidade entidade, CancellationToken cancellationToken = default);

    Task InserirAsync(IEnumerable<TEntidade> entidades, CancellationToken cancellationToken = default);

    Task EditarAsync(TEntidade entidade, CancellationToken cancellationToken = default);

    Task ExcluirAsync(TEntidade entidade, CancellationToken cancellationToken = default);

    Task ExcluirAsync(IEnumerable<TEntidade> entidades, CancellationToken cancellationToken = default);
}