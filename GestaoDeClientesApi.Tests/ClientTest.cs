using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Xunit;
using Bogus;
using GestaoDeClientesApi.Domain.Entities;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Bogus.Extensions.Brazil;
using System;
using System.Text;
using GestaoDeClientesApi.Services.Requests;
using DotNetEnv;

namespace GestaoDeClientesApi.Tests
{
    public class ClientTest
    {
        private HttpClient Initialize()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var webBuilder = new WebHostBuilder();
            webBuilder.UseConfiguration(configuration).UseStartup<Startup>();

            return new TestServer(webBuilder).CreateClient();
        }


        // CRIAÇÃO
        [Fact(DisplayName = "Criação de cliente")]
        public async Task<Cliente> CriaClienteTeste()
        {
            var faker = new Faker("pt_BR");

            var dateOfBirth = faker.Person.DateOfBirth;

            if(dateOfBirth.Year > 2004  || dateOfBirth.Year ==  2004 && dateOfBirth.DayOfYear > DateTime.Now.DayOfYear)
            {
                dateOfBirth = new DateTime(1998, dateOfBirth.Month, dateOfBirth.Day);
            }

            ClientePostRequest cliente = new()
            {
                Nome = faker.Person.FullName,
                Cpf = faker.Person.Cpf().Replace(".", "").Replace("-", ""),
                DataNascimento = dateOfBirth.ToString(),
                Email = faker.Person.Email,
            };

            var client = Initialize();
            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Cliente", content);

            var returnedCliente = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            return returnedCliente.cliente;
        }

        [Fact(DisplayName = "Criação de cliente menor de idade")]
        public async Task CriaClienteMenorDeIdadeTeste()
        {
            var faker = new Faker("pt_BR");

            var dateOfBirth = new DateTime(DateTime.Now.Year-14, 10, 10);

            ClientePostRequest cliente = new()
            {
                Nome = faker.Person.FullName,
                Cpf = faker.Person.Cpf().Replace(".", "").Replace("-", ""),
                DataNascimento = dateOfBirth.ToString(),
                Email = faker.Person.Email,
            };

            var client = Initialize();
            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Cliente", content);

            var returnedCliente = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("O cliente tem menos de 18 e sua criação não foi aceita.", returnedCliente.mensagem);

        }

        [Fact(DisplayName = "Criação de cliente com o mesmo CPF")]
        public async Task CriaClienteMesmoCpfTeste()
        {
            var faker = new Faker("pt_BR");

            var clienteResponse = await CriaClienteTeste();

            var dateOfBirth = new DateTime(DateTime.Now.Year - 25, 10, 10);

            ClientePostRequest cliente = new()
            {
                Nome = faker.Person.FullName,
                Cpf = clienteResponse.Cpf,
                DataNascimento = dateOfBirth.ToString(),
                Email = faker.Person.Email,
            };

            var client = Initialize();
            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Cliente", content);

            var returnedCliente = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("O CPF fornecido já está cadastrado em nosso sistema.", returnedCliente.mensagem);

        }

        [Fact(DisplayName = "Criação de cliente com o mesmo Email")]
        public async Task CriaClienteMesmoEmailTeste()
        {
            var faker = new Faker("pt_BR");

            var clienteResponse = await CriaClienteTeste();

            var dateOfBirth = new DateTime(DateTime.Now.Year - 25, 10, 10);

            ClientePostRequest cliente = new()
            {
                Nome = faker.Person.FullName,
                Cpf = faker.Person.Cpf().Replace(".", "").Replace("-", ""),
                DataNascimento = dateOfBirth.ToString(),
                Email = clienteResponse.Email,
            };

            var client = Initialize();
            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Cliente", content);

            var returnedCliente = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("O Email fornecido já está cadastrado em nosso sistema.", returnedCliente.mensagem);
        }


        

        // CONSULTAS
        [Fact(DisplayName = "Consulta de clientes")]
        public async Task<List<Cliente>> ConsultaClienteTeste()
        {
            var client = Initialize();

            var response = await client.GetAsync("api/Cliente");

            var clientes = JsonConvert.DeserializeObject<List<Cliente>>(response.Content.ReadAsStringAsync().Result);


            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(clientes);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(clientes.Count > 0);

            return clientes;
        }

        [Fact(DisplayName ="Consulta único cliente por Id")]
        public async Task ConsultaClientePorIdTeste()
        {
            var client = Initialize();
            var clientes = await ConsultaClienteTeste();

            Guid idCliente = clientes[0].IdCliente;

            var response = await client.GetAsync("api/Cliente/" + idCliente);

            var cliente = JsonConvert.DeserializeObject<Cliente>(response.Content.ReadAsStringAsync().Result);

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(cliente);
            Assert.Equal(idCliente, cliente.IdCliente);
        }

        [Fact(DisplayName ="Consulta cliente por Id inexistente")]
        public async Task ConsultaClientePorIdInexistenteTeste()
        {
            var client = Initialize();
            var response = await client.GetAsync("api/Cliente/" + Guid.NewGuid());
            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(resultado.cliente);
            Assert.Equal("Cliente não encontrado.", resultado.mensagem);

        }

        [Fact(DisplayName = "Consulta único cliente por Cpf")]
        public async Task ConsultaClientePorCpfTeste()
        {
            var client = Initialize();
            var clientes = await ConsultaClienteTeste();

            string cpf = clientes[0].Cpf;

            var response = await client.GetAsync("api/Cliente/cpf/" + cpf);

            var cliente = JsonConvert.DeserializeObject<Cliente>(response.Content.ReadAsStringAsync().Result);

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(cliente);
            Assert.Equal(cpf, cliente.Cpf);
        }

        [Fact(DisplayName = "Consulta cliente por Cpf inexistente")]
        public async Task ConsultaClientePorCpfInexistenteTeste()
        {
            var client = Initialize();
            var response = await client.GetAsync("api/Cliente/cpf/" + "0");
            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(resultado.cliente);
            Assert.Equal("Cliente não encontrado.", resultado.mensagem);

        }

        [Fact(DisplayName = "Consulta único cliente por Email")]
        public async Task ConsultaClientePorEmailTeste()
        {
            var client = Initialize();
            var clientes = await ConsultaClienteTeste();

            string email = clientes[0].Email;

            var response = await client.GetAsync("api/Cliente/email/" + email);

            var cliente = JsonConvert.DeserializeObject<Cliente>(response.Content.ReadAsStringAsync().Result);

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(cliente);
            Assert.Equal(email, cliente.Email);
        }

        [Fact(DisplayName = "Consulta cliente por Email inexistente")]
        public async Task ConsultaClientePorEmailInexistenteTeste()
        {
            var client = Initialize();
            var response = await client.GetAsync("api/Cliente/email/" + "a@a.a");
            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(resultado.cliente);
            Assert.Equal("Cliente não encontrado.", resultado.mensagem);

        }




        // ATUALIZAÇÃO
        [Fact(DisplayName = "Atualização de cliente")]
        public async Task AtualizaClienteTeste()
        {
            var client = Initialize();

            var faker = new Faker("pt_BR");

            var clienteResponse = await CriaClienteTeste();

            ClientePutRequest cliente = new()
            {
                IdCliente = clienteResponse.IdCliente,
                Nome = faker.Person.FullName,
                Cpf = faker.Person.Cpf().Replace(".", "").Replace("-", ""),
                DataNascimento = clienteResponse.DataNascimento.ToString(),
                Email = faker.Person.Email,
            };

            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("api/Cliente", content);

            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.True(response.IsSuccessStatusCode);
            Assert.True(resultado.cliente.IdCliente == clienteResponse.IdCliente);
            Assert.True(resultado.cliente.Nome == cliente.Nome);
            Assert.True(resultado.cliente.Cpf != clienteResponse.Cpf);
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Atualização de cliente com o mesmo Cpf")]
        public async Task AtualizaClienteComMesmoCpfTeste()
        {
            var client = Initialize();

            var faker = new Faker("pt_BR");

            var clienteResponse = await ConsultaClienteTeste();

            ClientePutRequest cliente = new()
            {
                IdCliente = clienteResponse[0].IdCliente,
                Nome = faker.Person.FullName,
                Cpf = clienteResponse[1].Cpf,
                DataNascimento = clienteResponse[0].DataNascimento.ToString(),
                Email = clienteResponse[0].Email,
            };

            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("api/Cliente", content);

            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Null(resultado.cliente);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("Já existe um cliente cadastrado com este CPF.", resultado.mensagem);
        }

        [Fact(DisplayName = "Atualização de cliente com o mesmo Email")]
        public async Task AtualizaClienteComMesmoEmailTeste()
        {
            var client = Initialize();

            var faker = new Faker("pt_BR");

            var clienteResponse = await ConsultaClienteTeste();

            ClientePutRequest cliente = new()
            {
                IdCliente = clienteResponse[0].IdCliente,
                Nome = faker.Person.FullName,
                Cpf = clienteResponse[0].Cpf,
                DataNascimento = clienteResponse[0].DataNascimento.ToString(),
                Email = clienteResponse[1].Email,
            };

            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("api/Cliente", content);

            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Null(resultado.cliente);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("Já existe um cliente cadastrado com este Email.", resultado.mensagem);
        }

        [Fact(DisplayName = "Atualização de cliente menor de idade")]
        public async Task AtualizaClienteComDataMenorDeIdadeTeste()
        {
            var client = Initialize();

            var faker = new Faker("pt_BR");

            var clienteResponse = await ConsultaClienteTeste();

            var dateOfBirth = new DateTime(DateTime.Now.Year - 14, 10, 10);


            ClientePutRequest cliente = new()
            {
                IdCliente = clienteResponse[0].IdCliente,
                Nome = faker.Person.FullName,
                Cpf = clienteResponse[0].Cpf,
                DataNascimento = dateOfBirth.ToString(),
                Email = clienteResponse[0].Email,
            };

            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("api/Cliente", content);

            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Null(resultado.cliente);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("O cliente atualizado não pode ser menor de idade.", resultado.mensagem);
        }

        [Fact(DisplayName = "Atualização de cliente inexistente")]
        public async Task AtualizaClienteInexistenteTeste()
        {
            var client = Initialize();

            var faker = new Faker("pt_BR");


            ClientePutRequest cliente = new()
            {
                IdCliente = Guid.NewGuid(),
                Nome = faker.Person.FullName,
                Cpf = faker.Person.Cpf().Replace(".", "").Replace("-", ""),
                DataNascimento = faker.Person.DateOfBirth.ToString(),
                Email = faker.Person.Email,
            };

            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("api/Cliente", content);

            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Null(resultado.cliente);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("O cliente com o IdCliente informado não foi encontrado.", resultado.mensagem);
        }




        // EXCLUSÃO
        [Fact(DisplayName ="Exclusão de cliente")]
        public async Task ExcluiClienteTeste()
        {
            var client = Initialize();

            var clientes = await ConsultaClienteTeste();
            var cliente = clientes[0];

            var response = await client.DeleteAsync("api/Cliente/" + cliente.IdCliente);
            
            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.True(response.IsSuccessStatusCode);
            Assert.Null(resultado.cliente);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.Equal("Cliente excluído com sucesso.", resultado.mensagem);

            var responseConsulta = await client.GetAsync("api/Cliente/" + cliente.IdCliente);
            var resultadoConsulta = JsonConvert.DeserializeObject<ResultadoCliente>(responseConsulta.Content.ReadAsStringAsync().Result);

            Assert.False(responseConsulta.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, responseConsulta.StatusCode);
            Assert.Null(resultadoConsulta.cliente);
            Assert.Equal("Cliente não encontrado.", resultadoConsulta.mensagem);

        }

        [Fact(DisplayName = "Exclusão de cliente inexistente")]
        public async Task ExcluiClienteInexistenteTeste()
        {
            var client = Initialize();

            var clientes = await ConsultaClienteTeste();
            var cliente = clientes[0];

            var response = await client.DeleteAsync("api/Cliente/" + Guid.NewGuid());

            var resultado = JsonConvert.DeserializeObject<ResultadoCliente>(response.Content.ReadAsStringAsync().Result);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(resultado.cliente);
            Assert.Equal("Cliente não encontrado.", resultado.mensagem);
        }
    }

    public class ResultadoCliente
    {
        public string mensagem { get; set; }

        public Cliente? cliente { get; set; }
    }

}