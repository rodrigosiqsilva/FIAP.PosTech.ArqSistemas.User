using FIAP.PosTech.ArqSistemas.UserAPI.Models;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Services
{
    public interface IUsuarioNotificacaoService
    {
        Task EnviarNotificacaoUsuario(Usuario usuario, string? correlationId);
    }
}
