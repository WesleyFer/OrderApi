using Order.Infra.Contextos;
using Microsoft.EntityFrameworkCore;

namespace Order.Api.Extensions;

internal sealed class AutoMigracao : IHostedService, IDisposable
{
    private bool _inicializado;
    private readonly SemaphoreSlim _semaphore;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AutoMigracao> _logger;

    public AutoMigracao(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AutoMigracao> logger,
        DbContext[] dbContexts)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _semaphore = new SemaphoreSlim(1, 1);
        _inicializado = false;
    }
    private async Task AplicarMigracaoAsync(DbContext[] dbContexts)
    {
        foreach (var context in dbContexts)
        {
            if (context is null || !context.Database.IsRelational())
                continue;

            await _semaphore.WaitAsync();
            try
            {
                var maxRetries = 5;
                var retries = 0;
                var migrationApplied = false;

                while (!migrationApplied && retries < maxRetries)
                {
                    try
                    {
                        if (context.Database.GetPendingMigrations().Any())
                        {
                            _logger.LogInformation($"Aplicando migrações para {context.GetType().Name} (tentativa {retries + 1})");
                            await context.Database.MigrateAsync();
                            _logger.LogInformation($"Migrações aplicadas para {context.GetType().Name}");
                        }
                        migrationApplied = true;
                    }
                    catch (Exception ex)
                    {
                        retries++;
                        _logger.LogWarning(ex, $"Erro ao aplicar migrações para {context.GetType().Name}, tentativa {retries} de {maxRetries}");
                        if (retries >= maxRetries)
                        {
                            _logger.LogError(ex, $"Falha ao aplicar migrações para {context.GetType().Name} após {maxRetries} tentativas");
                            throw;
                        }
                        await Task.Delay(5000);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_inicializado)
            return;

        _inicializado = true;
        using var scope = _serviceScopeFactory.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var contextos = new DbContext[]
        {
            serviceProvider.GetRequiredService<AuthContexto>(),
            serviceProvider.GetRequiredService<AppContexto>()
        };

        await AplicarMigracaoAsync(contextos);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("AutoMigração está sendo parada.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}
