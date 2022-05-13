using GestaoDeClientesApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeClientesApi.Infra.Data.Mappings
{
    public class ClienteMap : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("CLIENTE");

            builder.HasKey(c => c.IdCliente);

            
            builder.Property(c => c.IdCliente).HasColumnName("IDCLIENTE").IsRequired();

            builder.Property(c => c.Email).HasColumnName("EMAIL").IsRequired();
            builder.HasIndex(c => c.Email).IsUnique();

            builder.Property(c => c.Nome).HasColumnName("NOME").IsRequired();

            builder.Property(c => c.Cpf).HasColumnName("CPF").IsRequired();
            builder.HasIndex(c => c.Cpf).IsUnique();

            builder.Property(c => c.DataNascimento).HasColumnName("DATANASCIMENTO").IsRequired();
        }
    }
}
