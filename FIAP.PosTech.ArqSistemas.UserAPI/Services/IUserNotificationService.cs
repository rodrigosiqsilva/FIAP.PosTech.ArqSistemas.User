using FIAP.PosTech.ArqSistemas.UserAPI.Models;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Services
{
    public interface IUserNotificationService
    {
        Task SendNotificationUser(User user, string? correlationId);
    }
}
