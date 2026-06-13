using Confluent.Kafka;
using FIAP.PosTech.ArqSistemas.UserAPI.Models;
using FIAP.PosTech.ArqSistemas.UserAPI.Publisher;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Services
{
    public class UsuarioNotificarService : IUsuarioNotificarService
    {
        private readonly ILogger<UsuarioNotificarService> _logger;
        private readonly IConfiguration _configuration;

        public UsuarioNotificarService(ILogger<UsuarioNotificarService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task NotificarUsuario(Usuario usuario, string? correlationId)
        {
            string bootstrapServers = _configuration["KafkaConfig:BootstrapServers"];
            string topicName = _configuration["KafkaConfig:TopicName"];

            // Cria o evento
            var newEvent = new UserCreatedEvent(
                Usuario: usuario,
                CreatedAt: DateTime.UtcNow,
                CorrelationId: correlationId
            );

            using (var publisher = new UserEventPublisher(bootstrapServers, topicName))
            {
                try
                {
                    _logger.LogInformation("Publicando evento...");
                    await publisher.PublishUserCreatedEventAsync(newEvent);
                    _logger.LogInformation($"Evento publicado com sucesso! {newEvent}");

                }
                catch (Exception ex) 
                { 
                    _logger.LogError(ex, "Erro ao publicar evento");
                }
            }
        }
    }
}
