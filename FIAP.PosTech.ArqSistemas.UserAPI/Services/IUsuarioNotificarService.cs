using FIAP.PosTech.ArqSistemas.UserAPI.Models;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Services
{
    public interface IUsuarioNotificarService
    {
        Task NotificarUsuario(Usuario usuario, string? correlationId);
    }
}
