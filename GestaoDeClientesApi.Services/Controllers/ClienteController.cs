using GestaoDeClientesApi.Domain.Entities;
using GestaoDeClientesApi.Domain.Interfaces.Services;
using GestaoDeClientesApi.Services.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GestaoDeClientesApi.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Cliente> clientes = _clienteService.ConsultarClientes();

                return StatusCode(200, clientes);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensagem = e.Message });  ;
            }
        }

        [HttpPost]
        public IActionResult Post(ClientePostRequest request)
        {
            try
            { 
                if (!Int64.TryParse(request.Cpf, out Int64 _))
                {
                    return StatusCode(400, new { mensagem = "O CPF informado deve conter somente números" });
                }

                Cliente cliente = new()
                {
                    IdCliente = Guid.NewGuid(),
                    Nome = request.Nome,
                    Email = request.Email,
                    DataNascimento = DateTime.Parse(request.DataNascimento),
                    Cpf = request.Cpf,
                };

                _clienteService.CriarCliente(cliente);

                return StatusCode(201, new { mensagem = "Cliente cadastrado com sucesso", cliente }); ;
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("#"))
                {
                    var exception = e.Message.Replace("#", "");
                    var data = exception.Split("|");
                    int statusCode = int.Parse(data[0]);
                    string mensagem = data[1];

                    return StatusCode(statusCode, new { mensagem = mensagem });
                }
                return StatusCode(500, new { mensagem = e.Message }); ;
            }
        }

        [HttpPut]
        public IActionResult Put(ClientePutRequest request)
        {

            try
            {
                Cliente cliente = new()
                {
                    IdCliente= request.IdCliente,
                    Cpf = request.Cpf,
                    DataNascimento= DateTime.Parse(request.DataNascimento),
                    Email = request.Email,
                    Nome = request.Nome,
                };

                _clienteService.EditarCliente(cliente);

                return StatusCode(202, new { mensagem = "O Cliente foi atualizado com sucesso!", cliente });
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("#"))
                {
                    var exception = e.Message.Replace("#", "");
                    var data = exception.Split("|");
                    int statusCode = int.Parse(data[0]);
                    string mensagem = data[1];

                    return StatusCode(statusCode, new { mensagem = mensagem });
                }
                return StatusCode(500, new { mensagem = e.Message }); ;
            }
        }

        [HttpDelete("{idCliente}")]
        public IActionResult Delete(Guid idCliente)
        {
            try
            {
                _clienteService.ExcluirCliente(idCliente);

                return StatusCode(202, new { mensagem = "Cliente excluído com sucesso." });
            }
            catch (Exception e)
            {

                if (e.Message.StartsWith("#"))
                {
                    var exception = e.Message.Replace("#", "");
                    var data = exception.Split("|");
                    int statusCode = int.Parse(data[0]);
                    string mensagem = data[1];

                    return StatusCode(statusCode, new { mensagem = mensagem });
                }
                return StatusCode(500, new { mensagem = e.Message }); ;
            }
        }

        [HttpGet("{IdCliente}")]
        public IActionResult GetById(Guid IdCliente)
        {
            try
            {
                var cliente = _clienteService.ObterPorId(IdCliente);

                return StatusCode(200, cliente);
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("#"))
                {
                    var exception = e.Message.Replace("#", "");
                    var data = exception.Split("|");
                    int statusCode = int.Parse(data[0]);
                    string mensagem = data[1];

                    return StatusCode(statusCode, new { mensagem = mensagem });
                }
                return StatusCode(500, new { mensagem = e.Message }); ;
            }
        }

        [HttpGet("email/{email}")]
        public IActionResult GetByEmail(string email)
        {
            try
            {
                var cliente = _clienteService.ObterPorEmail(email);

                return StatusCode(200, cliente);
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("#"))
                {
                    var exception = e.Message.Replace("#", "");
                    var data = exception.Split("|");
                    int statusCode = int.Parse(data[0]);
                    string mensagem = data[1];

                    return StatusCode(statusCode, new { mensagem = mensagem });
                }
                return StatusCode(500, new { mensagem = e.Message }); ;
            }
        }


        [HttpGet("cpf/{cpf}")]
        public IActionResult GetByCpf(string cpf)
        {
            try
            {
                var cliente = _clienteService.ObterPorCpf(cpf);

                return StatusCode(200, cliente);
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("#"))
                {
                    var exception = e.Message.Replace("#", "");
                    var data = exception.Split("|");
                    int statusCode = int.Parse(data[0]);
                    string mensagem = data[1];

                    return StatusCode(statusCode, new { mensagem = mensagem });
                }
                return StatusCode(500, new { mensagem = e.Message }); ;
            }
        }
    }
}
