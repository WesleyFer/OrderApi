using Order.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infra.Contextos.AppConfig;

internal class PedidoConfig : AggregateRootConfig<Pedido>, IEntityTypeConfiguration<Pedido>
{
    public override void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CodigoPedido)
                 .IsRequired();

        builder.Property(p => p.CodigoCliente)
               .IsRequired();

        builder.HasMany(p => p.Produtos)
                  .WithOne()
                  .OnDelete(DeleteBehavior.Cascade);
    }
}
