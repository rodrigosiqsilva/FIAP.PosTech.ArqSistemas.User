using FIAP.PosTech.ArqSistemas.UserAPI.Models;
using FIAP.PosTech.ArqSistemas.UserAPI.DTOs;

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
        /// Autenticar um usuário pelo email e senha
        /// </summary>
        Usuario Autenticar(string email, string senha);

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        (bool Sucesso, string Mensagem, Usuario Usuario) Criar(Usuario usuario);

        /// <summary>
        /// Altera um usuário existente (partial update)
        /// Apenas o Id é obrigatório. Os demais campos são opcionais e serão atualizados somente se fornecidos.
        /// </summary>
        (bool Sucesso, string Mensagem, Usuario Usuario) Alterar(int id, AtualizarUsuarioDto usuarioAtualizado);

        /// <summary>
        /// Exclui um usuário pelo Id
        /// </summary>
        (bool Sucesso, string Mensagem) Excluir(int id);
    }
}
