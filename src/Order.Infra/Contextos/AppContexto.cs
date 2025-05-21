using Microsoft.EntityFrameworkCore;
using Order.Dominio.Contratos;
using Order.Dominio.Entidades;
using Order.Infra.Contextos.AppConfig;
using Flunt.Notifications;

namespace Order.Infra.Contextos;

public class AppContexto : DbContext, IQueryContexto
{
    public AppContexto(DbContextOptions<AppContexto> options) : base(options) { }

    public DbSet<Pedido> Pedidos { get; protected set; }

    public DbSet<Produto> Produtos { get; protected set; }

    public IQueryable<Pedido> QueryPedidos => Pedidos
        .Include(c => c.Produtos)
        .AsNoTrackingWithIdentityResolution()
        .AsQueryable();

    public IQueryable<Produto> QueryProdutos => Produtos
        .AsNoTrackingWithIdentityResolution()
        .AsQueryable();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContexto).Assembly, type =>
        {
            var mapeamentos = string.Join('.', typeof(PedidoConfig).FullName?.Split('.').SkipLast(1));
            return type.FullName?.StartsWith(mapeamentos) ?? false;
        });

        modelBuilder.Ignore<Notification>();
        modelBuilder.Ignore<Notifiable<Notification>>();
        modelBuilder.Ignore<Entidade>();
        modelBuilder.Ignore<IAggregateRoot>();
    }

}
