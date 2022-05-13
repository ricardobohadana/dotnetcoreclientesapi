using GestaoDeClientesApi.Domain.Entities;
using GestaoDeClientesApi.Domain.Interfaces.Repositories;
using GestaoDeClientesApi.Infra.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeClientesApi.Infra.Data.Repositories
{
    public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        private readonly SqlServerContext _sqlServerContext;

        public ClienteRepository(SqlServerContext sqlServerContext): base(sqlServerContext)
        {
            _sqlServerContext = sqlServerContext;
        }

        public Cliente ObterPorCpf(string cpf)
        {
            return _sqlServerContext.Cliente.Where(c => c.Cpf == cpf).FirstOrDefault();
        }

        public Cliente ObterPorEmail(string email)
        {
            return _sqlServerContext.Cliente.Where(c => c.Email == email).FirstOrDefault();
        }
    }
}
