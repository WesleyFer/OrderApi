using Order.Dominio.Contratos;
using Order.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infra.Contextos.AppConfig;

public class EntidadeConfig<T> : IEntityTypeConfiguration<T> where T : Entidade
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.DataCriacao)
               .ValueGeneratedOnAdd()
               .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(e => e.DataAtualizacao)
           .ValueGeneratedOnAddOrUpdate()
           .IsConcurrencyToken()
           .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}

public class AggregateRootConfig<T> : EntidadeConfig<T> where T : Entidade, IAggregateRoot
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);
    }
}