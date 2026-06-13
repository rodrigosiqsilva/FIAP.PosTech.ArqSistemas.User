using FIAP.PosTech.ArqSistemas.UserAPI.Models;
using FIAP.PosTech.ArqSistemas.UserAPI.Validators;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly List<Usuario> _usuarios;
        private readonly ILogger<UsuarioService> _logger;
        private int _proximoId = 6;

        public UsuarioService(ILogger<UsuarioService> logger)
        {
            _logger = logger;
            _usuarios = new List<Usuario>();
            InicializarDados();
        }

        /// <summary>
        /// Inicializa 5 registros fictícios para testes
        /// </summary>
        private void InicializarDados()
        {
            _usuarios.AddRange(new[]
            {
                new Usuario { Id = 1, Nome = "João Silva", Email = "joao@example.com", Senha = "SenhaSegura@123" },
                new Usuario { Id = 2, Nome = "Maria Santos", Email = "maria@example.com", Senha = "OutraSenha#456" },
                new Usuario { Id = 3, Nome = "Pedro Oliveira", Email = "pedro@example.com", Senha = "MaisSenha!789" },
                new Usuario { Id = 4, Nome = "Ana Costa", Email = "ana@example.com", Senha = "Senha@Teste#101" },
                new Usuario { Id = 5, Nome = "Carlos Mendes", Email = "carlos@example.com", Senha = "CarlosSenha$202" }
            });

            _logger.LogInformation("Dados iniciais de usuários carregados com sucesso. Total de registros: {TotalRegistros}", _usuarios.Count);
        }

        public List<Usuario> ObterTodos()
        {
            _logger.LogInformation("Obtendo todos os usuários. Total: {Total}", _usuarios.Count);
            return _usuarios.ToList();
        }

        public Usuario ObterPorId(int id)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                _logger.LogWarning("Usuário com Id {Id} não encontrado", id);
            }
            else
            {
                _logger.LogInformation("Usuário com Id {Id} encontrado: {Nome}", id, usuario.Nome);
            }
            return usuario;
        }

        public (bool Sucesso, string Mensagem, Usuario Usuario) Criar(Usuario usuario)
        {
            var erros = new List<string>();

            // Validar obrigatoriedade
            if (string.IsNullOrWhiteSpace(usuario.Nome))
                erros.Add("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(usuario.Email))
                erros.Add("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(usuario.Senha))
                erros.Add("Senha é obrigatória");

            // Validar email
            if (!string.IsNullOrWhiteSpace(usuario.Email) && !UsuarioValidador.ValidarEmail(usuario.Email))
                erros.Add("Formato de email inválido");

            // Validar email duplicado
            if (!string.IsNullOrWhiteSpace(usuario.Email) && _usuarios.Any(u => u.Email == usuario.Email))
                erros.Add("Email já cadastrado");

            // Validar senha segura
            if (!string.IsNullOrWhiteSpace(usuario.Senha) && !UsuarioValidador.ValidarSenhaSegura(usuario.Senha))
                erros.Add(UsuarioValidador.ObterMensagemErroSenha());

            if (erros.Count > 0)
            {
                var mensagem = string.Join("; ", erros);
                _logger.LogWarning("Erro ao criar usuário: {Erros}", mensagem);
                return (false, mensagem, null);
            }

            // Criar novo usuário com Id gerado
            var novoUsuario = new Usuario
            {
                Id = _proximoId++,
                Nome = usuario.Nome.Trim(),
                Email = usuario.Email.Trim(),
                Senha = usuario.Senha
            };

            _usuarios.Add(novoUsuario);
            _logger.LogInformation("Usuário criado com sucesso. Id: {Id}, Nome: {Nome}, Email: {Email}", 
                novoUsuario.Id, novoUsuario.Nome, novoUsuario.Email);

            return (true, "Usuário criado com sucesso", novoUsuario);
        }

        public (bool Sucesso, string Mensagem, Usuario Usuario) Alterar(int id, Usuario usuario)
        {
            var erros = new List<string>();

            // Validar Id obrigatório
            if (id <= 0)
                erros.Add("Id deve ser um número positivo");

            // Localizar usuário
            var usuarioExistente = _usuarios.FirstOrDefault(u => u.Id == id);
            if (usuarioExistente == null)
            {
                _logger.LogWarning("Erro ao alterar: Usuário com Id {Id} não encontrado", id);
                return (false, "Usuário não encontrado", null);
            }

            // Validar obrigatoriedade
            if (string.IsNullOrWhiteSpace(usuario.Nome))
                erros.Add("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(usuario.Email))
                erros.Add("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(usuario.Senha))
                erros.Add("Senha é obrigatória");

            // Validar email
            if (!string.IsNullOrWhiteSpace(usuario.Email) && !UsuarioValidador.ValidarEmail(usuario.Email))
                erros.Add("Formato de email inválido");

            // Validar email duplicado (excluindo o usuário atual)
            if (!string.IsNullOrWhiteSpace(usuario.Email) && 
                _usuarios.Any(u => u.Id != id && u.Email == usuario.Email))
                erros.Add("Email já cadastrado por outro usuário");

            // Validar senha segura
            if (!string.IsNullOrWhiteSpace(usuario.Senha) && !UsuarioValidador.ValidarSenhaSegura(usuario.Senha))
                erros.Add(UsuarioValidador.ObterMensagemErroSenha());

            if (erros.Count > 0)
            {
                var mensagem = string.Join("; ", erros);
                _logger.LogWarning("Erro ao alterar usuário {Id}: {Erros}", id, mensagem);
                return (false, mensagem, null);
            }

            // Atualizar usuário
            usuarioExistente.Nome = usuario.Nome.Trim();
            usuarioExistente.Email = usuario.Email.Trim();
            usuarioExistente.Senha = usuario.Senha;

            _logger.LogInformation("Usuário alterado com sucesso. Id: {Id}, Nome: {Nome}, Email: {Email}", 
                usuarioExistente.Id, usuarioExistente.Nome, usuarioExistente.Email);

            return (true, "Usuário alterado com sucesso", usuarioExistente);
        }

        public (bool Sucesso, string Mensagem) Excluir(int id)
        {
            // Validar Id
            if (id <= 0)
                return (false, "Id deve ser um número positivo");

            // Localizar usuário
            var usuarioExistente = _usuarios.FirstOrDefault(u => u.Id == id);
            if (usuarioExistente == null)
            {
                _logger.LogWarning("Erro ao excluir: Usuário com Id {Id} não encontrado", id);
                return (false, "Usuário não encontrado");
            }

            // Remover usuário
            _usuarios.Remove(usuarioExistente);
            _logger.LogInformation("Usuário excluído com sucesso. Id: {Id}, Nome: {Nome}", id, usuarioExistente.Nome);

            return (true, "Usuário excluído com sucesso");
        }
    }
}
