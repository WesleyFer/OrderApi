using Order.Dominio.Contratos;
using Order.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Order.Infra.Repositorios;

public sealed class UnitOfWork<TContexto> : IUnitOfWork<TContexto>, IDisposable
    where TContexto : IQueryContexto
{
    private readonly DbContext _dbContext;
    private readonly ConcurrentDictionary<string, object> _repositorios;
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed = false;
    private readonly ILogger<UnitOfWork<TContexto>> _logger;

    public UnitOfWork(TContexto dbContext, IServiceProvider serviceProvider, ILogger<UnitOfWork<TContexto>> logger)
    {
        _dbContext = dbContext as DbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _repositorios = new ConcurrentDictionary<string, object>();
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                int result = await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Transaction committed successfully.");
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred during transaction. Rolling back.");
                throw;
            }
        });
    }

    public async Task RollbackAsync()
    {
        _dbContext.ChangeTracker.Clear();
        await Task.CompletedTask;
    }

    public IRepositorio<TEntidade> Repositorio<TEntidade>() where TEntidade : Entidade, IAggregateRoot
    {
        var nomeTipoEntidade = typeof(TEntidade).FullName ?? typeof(TEntidade).Name;

        if (_repositorios.TryGetValue(nomeTipoEntidade, out var repoExistente))
        {
            return (IRepositorio<TEntidade>)repoExistente;
        }

        var repositorio = _serviceProvider.GetService<IRepositorio<TEntidade>>();

        if (repositorio == null)
        {
            repositorio = new Repositorio<TEntidade>(_dbContext);
        }

        _repositorios.TryAdd(nomeTipoEntidade, repositorio);
        return repositorio;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }
    }
}