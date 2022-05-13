using System.ComponentModel.DataAnnotations;

namespace GestaoDeClientesApi.Services.Requests
{
    public class ClientePostRequest
    {

        [Required(ErrorMessage = "Por favor, informe o nome do cliente.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Por favor, informe a data de nascimento do cliente.")]
        public string DataNascimento { get; set; }

        [EmailAddress(ErrorMessage = "Por favor, informe um email válido para o cliente.")]
        [Required(ErrorMessage = "Por favor, informe o email do cliente.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Por favor, informe o cnpj da empresa.")]
        public string Cpf { get; set; }
    }
}
