using Order.Dominio.Contratos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Order.Dominio.Entidades;

namespace Order.Infra.Repositorios;


public class Repositorio<TEntidade> : IRepositorio<TEntidade> where TEntidade : Entidade, IAggregateRoot
{
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntidade> _dbSet;

    public Repositorio(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntidade>();
    }

    public async Task<IEnumerable<TEntidade>> BuscarAsync(Expression<Func<TEntidade, bool>>? filtro = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntidade> query = _dbContext.Set<TEntidade>();

        if (filtro != null)
        {
            query = query.Where(filtro);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntidade>> BuscarAsync(Expression<Func<TEntidade, bool>>? filtro = null, CancellationToken cancellationToken = default, params Expression<Func<TEntidade, object>>[] includes)
    {
        IQueryable<TEntidade> query = _dbContext.Set<TEntidade>();

        if (filtro != null)
        {
            query = query.Where(filtro);
        }

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.ToListAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
        GC.SuppressFinalize(this);
    }

    public Task EditarAsync(TEntidade entidade, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entidade);
        return Task.CompletedTask;
    }

    public Task ExcluirAsync(TEntidade entidade, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entidade);
        return Task.CompletedTask;
    }

    public Task ExcluirAsync(IEnumerable<TEntidade> entidades, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entidades);
        return Task.CompletedTask;
    }

    public async Task InserirAsync(TEntidade entidade, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entidade, cancellationToken);
    }

    public async Task InserirAsync(IEnumerable<TEntidade> entidades, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entidades, cancellationToken);
    }
}
