# FIAP.PosTech.ArqSistemas.UserAPI

API Web desenvolvida em **.NET 8** utilizando os princípios de **Domain-Driven Design (DDD)** e arquitetura de microsserviços. A aplicação expõe endpoints REST para operações síncronas de gerenciamento de perfis, controle de autenticação e integra-se de forma assíncrona ao **Apache Kafka** para mensageria e difusão de eventos.

Ela é o componente central de gerenciamento de identidade e autenticação para a plataforma de jogos digitais. A aplicação foi projetada seguindo práticas modernas de arquitetura de software para garantir segurança, validação rigorosa de dados de acesso e a sincronização do ciclo de vida dos usuários com os demais microsserviços do ecossistema.

Essa API atua como a barreira e o motor de segurança por trás da experiência do usuário, gerenciando desde a criação de contas com políticas de senhas complexas, geração de tokens criptográficos **JWT (JSON Web Tokens)**, até a notificação em tempo real de novos registros na base de dados através de eventos orientados a mensagens.

---

## 🛠️ Tecnologias e Frameworks

* **Runtime:** .NET 8.0 SDK
* **Segurança e Autenticação:** JWT Bearer Token com Autorização baseada em Funções (`Admin`/`User`)
* **Documentação:** Swagger / OpenAPI 3
* **Mensageria:** Confluent Kafka Client (Event-Driven Publisher)
* **Containers & Orquestração:** Docker & Kubernetes

---

## 🎯 Escopo e Funcionamento Interno

A aplicação centraliza as regras de negócio associadas aos perfis de acesso da plataforma, executando os seguintes fluxos arquiteturais:

1. **Autenticação Segura (`AutenticationController`):** Valida as credenciais (`Email` e `Senha`) e emite chaves temporárias criptografadas de acesso (Tokens JWT válidos por 30 minutos).
2. **Políticas de Validação Estruturada (`UserValidador`):** Toda inserção ou modificação de cadastro passa por rotinas que exigem o formato correto de e-mail e políticas de senha forte (mínimo de 8 caracteres, contendo letras, números e caracteres especiais).
3. **Controle de Acesso Fino (RBAC):** Protege recursos administrativos sensíveis (como listagem geral e deleção) através de atributos anotados (`[Authorize(Roles = "Admin")]`), inspecionando as claims do Token JWT.
4. **Publicação de Eventos (`UserNotificationService` & `UserEventPublisher`):** Ao consolidar a criação de um usuário com sucesso, o sistema monta um evento técnico unificado (`UserCreatedEvent`), acopla o `CorrelationId` recebido no request e o publica no tópico mapeamento do Apache Kafka (`UserCreated`), notificando os microsserviços dependentes.

---

## 🚀 Como Executar e Acessar o Swagger

### Pré-requisitos
* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) instalado.
* Infraestrutura de mensageria (Kafka) rodando localmente ou via container.

---

### Repositório do Ecossistema
Você precisará clonar o seguintes repositório do projeto:

| Repositório | Link para Clone |
| :--- | :--- |
| **User API** | `https://github.com/rodrigosiqsilva/FIAP.PosTech.ArqSistemas.User.git` |

### 🧪 Estratégia de Testes rápidos (.http)
Para fins de validação rápida sem necessidade de abrir a interface do navegador, o repositório disponibiliza o arquivo FIAP.PosTech.ArqSistemas.UserAPI.Testes.http na raiz do projeto.

Ele contém requisições HTTP pré-configuradas e prontas para execução utilizando o recurso de HTTP Client.

Como usar: Abra o arquivo utilizando a extensão REST Client (no Visual Studio Code) ou a ferramenta nativa de HTTP Client do Visual Studio e clique em Send Request diretamente acima do endpoint desejado.

### 📂 Estrutura de Pastas Obrigatória
Para que os arquivos de orquestração local (Docker Compose) referenciem os projetos corretamente, você **deve** respeitar a seguinte estrutura de diretórios no seu disco:

Veja um exemplo através da imagem: https://github.com/rodrigosiqsilva/FIAP.PosTech.ArqSistemas.Orchestrator/blob/main/Estrututa%20pastas.png

```text
C:\Sistemas\FIAP\     
├── FIAP.PosTech.ArqSistemas.Catalog/  
├── FIAP.PosTech.ArqSistemas.User/   <- (Arquivos desse repositório mencionados aqui)
├── FIAP.PosTech.ArqSistemas.Notification/
└── FIAP.PosTech.ArqSistemas.Payments/