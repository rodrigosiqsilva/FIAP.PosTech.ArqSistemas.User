using FIAP.PosTech.ArqSistemas.UserAPI.Models;
using FIAP.PosTech.ArqSistemas.UserAPI.DTOs;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        List<User> ObterTodos();

        /// <summary>
        /// Obtém um usuário pelo Id
        /// </summary>
        User ObterPorId(int id);

        /// <summary>
        /// Autenticar um usuário pelo email e senha
        /// </summary>
        User Autenticar(string email, string senha);

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        (bool Sucesso, string Mensagem, User User) Criar(User user);

        /// <summary>
        /// Altera um usuário existente (partial update)
        /// Apenas o Id é obrigatório. Os demais campos são opcionais e serão atualizados somente se fornecidos.
        /// </summary>
        (bool Sucesso, string Mensagem, User User) Alterar(int id, AtualizarUserDto userAtualizado);

        /// <summary>
        /// Exclui um usuário pelo Id
        /// </summary>
        (bool Sucesso, string Mensagem) Excluir(int id);
    }
}
