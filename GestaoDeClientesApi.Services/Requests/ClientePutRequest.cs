using System.ComponentModel.DataAnnotations;

namespace GestaoDeClientesApi.Services.Requests
{
    public class ClientePutRequest
    {

        [Required( ErrorMessage = "Por favor, informe o IdCliente.")]
        public Guid IdCliente { get; set; }

        [Required( ErrorMessage = "Por favor, informe o nome do cliente.")]
        public string Nome{ get; set; }

        [Required( ErrorMessage = "Por favor, informe o Email.")]
        public string Email { get; set; }

        [Required( ErrorMessage = "Por favor, informe o Cpf.")]
        public string Cpf { get; set; }

        [Required( ErrorMessage = "Por favor, informe a Data de nascimento.")]
        public string DataNascimento { get; set; }
    }
}
