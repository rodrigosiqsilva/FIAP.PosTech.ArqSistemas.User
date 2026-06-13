using FIAP.PosTech.ArqSistemas.UserAPI.Models;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Services
{
    public interface IUsuarioService
    {
        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        List<Usuario> ObterTodos();

        /// <summary>
        /// Obtém um usuário pelo Id
        /// </summary>
        Usuario ObterPorId(int id);

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        (bool Sucesso, string Mensagem, Usuario Usuario) Criar(Usuario usuario);

        /// <summary>
        /// Altera um usuário existente
        /// </summary>
        (bool Sucesso, string Mensagem, Usuario Usuario) Alterar(int id, Usuario usuario);

        /// <summary>
        /// Exclui um usuário pelo Id
        /// </summary>
        (bool Sucesso, string Mensagem) Excluir(int id);
    }
}
