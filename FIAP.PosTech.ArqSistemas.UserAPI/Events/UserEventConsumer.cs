namespace FIAP.PosTech.ArqSistemas.UserAPI.Publisher
{
    using Confluent.Kafka;
    using System.Text.Json;

    namespace FIAP.PosTech.ArqSistemas.UserAPI.Consumer
    {
        public class UserEventConsumer : IDisposable
        {
            private readonly IConsumer<string, string> _consumer;
            private readonly string _topicName;

            public UserEventConsumer(string bootstrapServers, string topicName, string groupId)
            {
                _topicName = topicName;

                // Configuração do consumidor
                var config = new ConsumerConfig
                {
                    BootstrapServers = bootstrapServers,
                    GroupId = groupId, // O GroupId é obrigatório para consumidores Kafka
                    AutoOffsetReset = AutoOffsetReset.Earliest, // Lê do início se não houver commit anterior
                    EnableAutoCommit = true // Commita automaticamente o offset após o consumo
                };

                // Constrói o consumidor definindo a Chave (Key) e o Valor (Value) como strings
                _consumer = new ConsumerBuilder<string, string>(config).Build();
            }

            /// <summary>
            /// Inicia o loop de consumo de forma assíncrona.
            /// </summary>
            /// <param name="cancellationToken">Token para permitir o cancelamento gracioso do loop.</param>
            public Task StartConsumingAsync(CancellationToken cancellationToken)
            {
                // Assina o tópico
                _consumer.Subscribe(_topicName);

                // Roda o loop de consumo em uma Thread separada para não travar a aplicação principal
                return Task.Run(() =>
                {
                    try
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            try
                            {
                                // O método Consume bloqueia até que uma mensagem esteja disponível ou o token seja cancelado
                                var consumeResult = _consumer.Consume(cancellationToken);

                                if (consumeResult != null)
                                {
                                    // Desserializa o JSON de volta para o objeto original
                                    var userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(consumeResult.Message.Value);

                                    // =========================================================
                                    // TODO: Adicione sua lógica de negócio aqui
                                    // Exemplo: Salvar no banco, chamar outra API, logar, etc.
                                    // =========================================================

                                    Console.WriteLine($"[Kafka Consumer] Processando Usuário ID: {consumeResult.Message.Key} | Criado em: {userEvent?.CreatedAt}");
                                }
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"[Kafka Consumer] Erro ao consumir mensagem: {e.Error.Reason}");
                                // Opcional: Implementar Dead Letter Queue (DLQ) aqui
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Exceção esperada ao cancelar o CancellationToken (desligamento gracioso)
                        Console.WriteLine("[Kafka Consumer] Encerramento solicitado.");
                    }
                    finally
                    {
                        // Fecha o consumer e garante que os offsets sejam enviados para o broker
                        _consumer.Close();
                    }
                }, cancellationToken);
            }

            public void Dispose()
            {
                _consumer?.Dispose();
            }
        }
    }
}
