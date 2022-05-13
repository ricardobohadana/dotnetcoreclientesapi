using GestaoDeClientesApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeClientesApi.Domain.Interfaces.Services
{
    public interface IClienteService
    {
        void CriarCliente(Cliente cliente);

        void EditarCliente(Cliente cliente);

        void ExcluirCliente(Guid idCliente);

        List<Cliente> ConsultarClientes();

        Cliente ObterPorId(Guid idCliente);

        Cliente ObterPorEmail(string email);

        Cliente ObterPorCpf(string cpf);

    }
}
