using Order.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infra.Contextos.AppConfig;

internal class ProdutoConfig : EntidadeConfig<Produto>, IEntityTypeConfiguration<Produto>
{
    public override void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(p => p.Quantidade)
               .IsRequired();

        builder.Property(p => p.Preco)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

    }
}
