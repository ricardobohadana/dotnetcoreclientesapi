using GestaoDeClientesApi.Domain.Entities;
using GestaoDeClientesApi.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeClientesApi.Domain.Interfaces.Repositories
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
        Cliente ObterPorCpf(string cpf);
        Cliente ObterPorEmail(string email);
    }
}
