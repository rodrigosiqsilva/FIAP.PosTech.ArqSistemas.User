using FIAP.PosTech.ArqSistemas.UserAPI.DTOs;
using FIAP.PosTech.ArqSistemas.UserAPI.Models;
using FIAP.PosTech.ArqSistemas.UserAPI.Publisher;
using FIAP.PosTech.ArqSistemas.UserAPI.Validators;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _user;
        private readonly ILogger<UserService> _logger;
        private int _proximoId = 6;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
            _user = new List<User>();
            InicializarDados();
        }

        /// <summary>
        /// Inicializa 5 registros fictícios para testes
        /// </summary>
        private void InicializarDados()
        {
            _user.AddRange(new[]
            {
                new User { Id = 1, Nome = "João Silva", Email = "joao@example.com", Senha = "SenhaSegura@123" },
                new User { Id = 2, Nome = "Maria Santos", Email = "maria@example.com", Senha = "OutraSenha#456" },
                new User { Id = 3, Nome = "Pedro Oliveira", Email = "pedro@example.com", Senha = "MaisSenha!789" },
                new User { Id = 4, Nome = "Ana Costa", Email = "ana@example.com", Senha = "Senha@Teste#101" },
                new User { Id = 5, Nome = "Carlos Mendes", Email = "carlos@example.com", Senha = "CarlosSenha$202" }
            });

            _logger.LogInformation("Dados iniciais de usuários carregados com sucesso. Total de registros: {TotalRegistros}", _user.Count);
        }

        public List<User> ObterTodos()
        {
            _logger.LogInformation("Obtendo todos os usuários. Total: {Total}", _user.Count);
            return _user.ToList();
        }

        public User ObterPorId(int id)
        {
            var user = _user.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("Usuário com Id {Id} não encontrado", id);
            }
            else
            {
                _logger.LogInformation("Usuário com Id {Id} encontrado: {Nome}", id, user.Nome);
            }
            return user;
        }

        public User Autenticar(string email, string senha)
        {
            var user = _user.FirstOrDefault(u => u.Email == email && u.Senha == senha);
            if (user == null)
            {
                _logger.LogWarning("Usuário com Email {Email} não encontrado. Verifique se a senha e o e-mail estão corretos", email);
            }
            else
            {
                _logger.LogInformation("Usuário com Email {Email} encontrado: {Nome}", email, user.Nome);
            }
            return user;
        }

        public (bool Sucesso, string Mensagem, User User) Criar(User user)
        {
            var erros = new List<string>();

            // Validar obrigatoriedade
            if (string.IsNullOrWhiteSpace(user.Nome))
                erros.Add("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(user.Email))
                erros.Add("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(user.Senha))
                erros.Add("Senha é obrigatória");

            // Validar email
            if (!string.IsNullOrWhiteSpace(user.Email) && !UserValidador.ValidarEmail(user.Email))
                erros.Add("Formato de email inválido");

            // Validar email duplicado
            if (!string.IsNullOrWhiteSpace(user.Email) && _user.Any(u => u.Email == user.Email))
                erros.Add("Email já cadastrado");

            // Validar senha segura
            if (!string.IsNullOrWhiteSpace(user.Senha) && !UserValidador.ValidarSenhaSegura(user.Senha))
                erros.Add(UserValidador.ObterMensagemErroSenha());

            if (erros.Count > 0)
            {
                var mensagem = string.Join("; ", erros);
                _logger.LogWarning("Erro ao criar usuário: {Erros}", mensagem);
                return (false, mensagem, null);
            }

            // Criar novo usuário com Id gerado
            var novoUser= new User
            {
                Id = _proximoId++,
                Nome = user.Nome.Trim(),
                Email = user.Email.Trim(),
                Senha = user.Senha
            };

            _user.Add(novoUser);
            _logger.LogInformation("Usuário criado com sucesso. Id: {Id}, Nome: {Nome}, Email: {Email}", 
                novoUser.Id, novoUser.Nome, novoUser.Email);

            return (true, "Usuário criado com sucesso", novoUser);
        }

        public (bool Sucesso, string Mensagem, User User) Alterar(int id, AtualizarUserDto userAtualizado)
        {
            var erros = new List<string>();

            // Validar Id obrigatório
            if (id <= 0)
                erros.Add("Id deve ser um número positivo");

            // Localizar usuário
            var userExistente = _user.FirstOrDefault(u => u.Id == id);
            if (userExistente == null)
            {
                _logger.LogWarning("Erro ao alterar: Usuário com Id {Id} não encontrado", id);
                return (false, "Usuário não encontrado", null);
            }

            // Validar e atualizar Nome (se fornecido)
            if (!string.IsNullOrWhiteSpace(userAtualizado.Nome))
            {
                userExistente.Nome = userAtualizado.Nome.Trim();
                _logger.LogInformation("Campo Nome atualizado para o usuário Id {Id}", id);
            }

            // Validar e atualizar Email (se fornecido)
            if (!string.IsNullOrWhiteSpace(userAtualizado.Email))
            {
                // Validar formato de email
                if (!UserValidador.ValidarEmail(userAtualizado.Email))
                {
                    erros.Add("Formato de email inválido");
                }
                // Validar email duplicado (excluindo o usuário atual)
                else if (_user.Any(u => u.Id != id && u.Email == userAtualizado.Email))
                {
                    erros.Add("Email já cadastrado por outro usuário");
                }
                else
                {
                    userExistente.Email = userAtualizado.Email.Trim();
                    _logger.LogInformation("Campo Email atualizado para o usuário Id {Id}", id);
                }
            }

            // Validar e atualizar Senha (se fornecido)
            if (!string.IsNullOrWhiteSpace(userAtualizado.Senha))
            {
                // Validar senha segura
                if (!UserValidador.ValidarSenhaSegura(userAtualizado.Senha))
                {
                    erros.Add(UserValidador.ObterMensagemErroSenha());
                }
                else
                {
                    userExistente.Senha = userAtualizado.Senha;
                    _logger.LogInformation("Campo Senha atualizado para o usuário Id {Id}", id);
                }
            }

            if (erros.Count > 0)
            {
                var mensagem = string.Join("; ", erros);
                _logger.LogWarning("Erro ao alterar usuário {Id}: {Erros}", id, mensagem);
                return (false, mensagem, null);
            }

            _logger.LogInformation("Usuário alterado com sucesso. Id: {Id}, Nome: {Nome}, Email: {Email}", 
                userExistente.Id, userExistente.Nome, userExistente.Email);

            return (true, "Usuário alterado com sucesso", userExistente);
        }

        public (bool Sucesso, string Mensagem) Excluir(int id)
        {
            // Validar Id
            if (id <= 0)
                return (false, "Id deve ser um número positivo");

            // Localizar usuário
            var userExistente = _user.FirstOrDefault(u => u.Id == id);
            if (userExistente == null)
            {
                _logger.LogWarning("Erro ao excluir: Usuário com Id {Id} não encontrado", id);
                return (false, "Usuário não encontrado");
            }

            // Remover usuário
            _user.Remove(userExistente);
            _logger.LogInformation("Usuário excluído com sucesso. Id: {Id}, Nome: {Nome}", id, userExistente.Nome);

            return (true, "Usuário excluído com sucesso");
        }

    }
}
