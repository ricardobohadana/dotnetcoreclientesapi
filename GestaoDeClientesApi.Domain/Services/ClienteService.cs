using GestaoDeClientesApi.Domain.Entities;
using GestaoDeClientesApi.Domain.Interfaces.Repositories;
using GestaoDeClientesApi.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeClientesApi.Domain.Services
{
    public class ClienteService : IClienteService
    {

        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        private int GetAge(Cliente cliente)
        {
            DateTime now = DateTime.Today;
            DateTime birthday = cliente.DataNascimento;

            int age = now.Year - birthday.Year;

            if (now.Month < birthday.Month || (now.Month == birthday.Month && now.Day < birthday.Day))//not had bday this year yet
                age--;

            return age;
        }

        public void CriarCliente(Cliente cliente)
        {
            // Checando se o usuário é maior de idade
            

            if (GetAge(cliente) < 18)
            {
                throw new Exception("#400|O cliente tem menos de 18 e sua criação não foi aceita.");
            }

            // Checando se o CPF e Email não estão cadastrados
            if (_clienteRepository.ObterPorCpf(cliente.Cpf) != null)
                throw new Exception("#409|O CPF fornecido já está cadastrado em nosso sistema."); 
            
            if (_clienteRepository.ObterPorEmail(cliente.Email) != null)
                throw new Exception("#409|O Email fornecido já está cadastrado em nosso sistema.");
    
            _clienteRepository.Inserir(cliente);
                
        }

        public void EditarCliente(Cliente cliente)
        {
            var clienteDb = _clienteRepository.ObterPorId(cliente.IdCliente);

            if(clienteDb == null)
                throw new Exception("#400|O cliente com o IdCliente informado não foi encontrado.");

            var clienteCpf = _clienteRepository.ObterPorCpf(cliente.Cpf);
            if (clienteCpf != null && clienteCpf.IdCliente != cliente.IdCliente)
                throw new Exception("#409|Já existe um cliente cadastrado com este CPF.");

            var clienteEmail = _clienteRepository.ObterPorEmail(cliente.Email);
            if (clienteEmail != null && clienteEmail.IdCliente != cliente.IdCliente)
                throw new Exception("#409|Já existe um cliente cadastrado com este Email.");

            if(GetAge(cliente) < 18)
                throw new Exception("#400|O cliente atualizado não pode ser menor de idade.");

            clienteDb.Nome = cliente.Nome;
            clienteDb.Email = cliente.Email;
            clienteDb.DataNascimento = cliente.DataNascimento;
            clienteDb.Cpf = cliente.Cpf;

            _clienteRepository.Alterar(clienteDb);
        }

        public void ExcluirCliente(Guid idCliente)
        {
            var cliente = _clienteRepository.ObterPorId(idCliente);

            if (cliente == null)
                throw new Exception("#400|Cliente não encontrado.");

            _clienteRepository.Excluir(cliente);

        }

        public List<Cliente> ConsultarClientes()
        {
            return _clienteRepository.Consultar();
        }

        public Cliente ObterPorId(Guid idCliente)
        {
            var cliente = _clienteRepository.ObterPorId(idCliente);

            if (cliente == null)
                throw new Exception("#400|Cliente não encontrado.");

            return cliente;
        }

        public Cliente ObterPorEmail(string email)
        {
            var cliente = _clienteRepository.ObterPorEmail(email);

            if (cliente == null)
                throw new Exception("#400|Cliente não encontrado.");

            return cliente;
        }

        public Cliente ObterPorCpf(string cpf)
        {
            var cliente = _clienteRepository.ObterPorCpf(cpf);

            if (cliente == null)
                throw new Exception("#400|Cliente não encontrado.");

            return cliente;
        }
    }
}
