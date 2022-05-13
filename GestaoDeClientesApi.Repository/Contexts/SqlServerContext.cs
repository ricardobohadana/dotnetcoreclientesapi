using GestaoDeClientesApi.Domain.Entities;
using GestaoDeClientesApi.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeClientesApi.Infra.Data.Contexts
{
    public class SqlServerContext : DbContext
    {
        public SqlServerContext (DbContextOptions<SqlServerContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClienteMap());
        }

        public DbSet<Cliente> Cliente { get; set; }
    }
}
