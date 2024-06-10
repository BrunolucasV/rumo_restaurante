using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace restaurante.Pages.clientes
{
    // A classe index_cliente representa a página Razor para exibir e adicionar clientes
    public class index_cliente : PageModel
    {
        // Logger para registrar logs e mensagens de erro
        private readonly ILogger<index_cliente> _logger;

        // Construtor que inicializa o logger
        public index_cliente(ILogger<index_cliente> logger)
        {
            _logger = logger;
        }

        // Lista para armazenar os clientes lidos do banco de dados
        public List<CustomerInfo> clienteListas { get; set; } = new List<CustomerInfo>();

        // Propriedade ligada ao formulário para adicionar um novo cliente
        [BindProperty]
        public CustomerInfo NewCustomer { get; set; }

        // Método chamado na requisição GET para carregar os dados dos clientes
        public void OnGet()
        {
            try
            {
                // String de conexão com o banco de dados
                string connectionString = "server=.;Database=restaurante;Trusted_Connection=true;TrustServerCertificate=true";

                // Abrindo a conexão com o banco de dados
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Query SQL para selecionar todos os clientes
                    string sql = "SELECT cpf_cliente, nome, email, tef, senha FROM cliente";

                    // Executando o comando SQL
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Lendo os dados dos clientes
                            while (reader.Read())
                            {
                                // Criando um novo objeto CustomerInfo para cada cliente
                                CustomerInfo customerInfo = new CustomerInfo
                                {
                                    cpf = reader.GetString(0),    // Lendo o CPF do cliente
                                    nome = reader.GetString(1),   // Lendo o nome do cliente
                                    email = reader.GetString(2),  // Lendo o email do cliente
                                    tef = reader.GetString(3),    // Lendo o telefone do cliente
                                    senha = reader.GetString(4)   // Lendo a senha do cliente
                                };

                                // Adicionando o cliente à lista
                                clienteListas.Add(customerInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Registrando qualquer erro que ocorrer durante o acesso ao banco de dados
                _logger.LogError("Erro ao acessar o banco de dados: " + ex.Message);
            }
        }

        // Método chamado na requisição POST para adicionar um novo cliente
        public IActionResult OnPost()
        {
            // Verifica se o modelo é válido
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // String de conexão com o banco de dados
                string connectionString = "server=.;Database=restaurante;Trusted_Connection=true;TrustServerCertificate=true";

                // Abrindo a conexão com o banco de dados
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Query SQL para inserir um novo cliente
                    string sql = "INSERT INTO cliente (cpf_cliente, nome, email, tef, senha) VALUES (@cpf_cliente, @nome, @email, @tef, @senha)";

                    // Executando o comando SQL para inserir o novo cliente
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Adicionando parâmetros ao comando SQL
                        command.Parameters.AddWithValue("@cpf_cliente", NewCustomer.cpf);
                        command.Parameters.AddWithValue("@nome", NewCustomer.nome);
                        command.Parameters.AddWithValue("@email", NewCustomer.email);
                        command.Parameters.AddWithValue("@tef", NewCustomer.tef);
                        command.Parameters.AddWithValue("@senha", NewCustomer.senha);

                        // Executando o comando
                        command.ExecuteNonQuery();
                    }
                }

                // Redireciona para a mesma página após a inserção
                return RedirectToPage("./index_cliente");
            }
            catch (Exception ex)
            {
                // Registrando qualquer erro que ocorrer durante a inserção no banco de dados
                _logger.LogError("Erro ao inserir no banco de dados: " + ex.Message);
                return Page();
            }
        }
    }

    // Classe que representa as informações do cliente
    public class CustomerInfo
    {
        public string cpf { get; set; } = "";   // CPF do cliente
        public string nome { get; set; } = "";  // Nome do cliente
        public string email { get; set; } = ""; // Email do cliente
        public string tef { get; set; } = "";   // Telefone do cliente
        public string senha { get; set; } = ""; // Senha do cliente
    }
}
