using FIAP.PosTech.ArqSistemas.UserAPI.DTOs;
using FIAP.PosTech.ArqSistemas.UserAPI.Models;
using FIAP.PosTech.ArqSistemas.UserAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IUserNotificationService _userNotificationService;

        public UserController(IUserService userService, ILogger<UserController> logger, IUserNotificationService userNotificationService)
        {
            _userService = userService;
            _logger = logger;
            _userNotificationService = userNotificationService;
        }

        /// <summary>
        /// Obtém o CorrelationId do contexto HTTP
        /// </summary>
        private string? GetCorrelationId() => HttpContext.Items["CorrelationId"]?.ToString();

        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        /// <returns>Lista de todos os usuários</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<List<User>>> ObterTodos()
        {
            try
            {
                var users = _userService.ObterTodos();
                var response = ApiResponse<List<User>>.SucessoList(users, $"Total de {users.Count} usuários encontrados");
                response.CorrelationId = GetCorrelationId();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os usuários");
                var response = ApiResponse<List<User>>.Erro(ex.Message, "Erro ao obter usuários");
                response.CorrelationId = GetCorrelationId();
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Obtém um usuário pelo Id
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <returns>Usuário encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<User>> ObterPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    var errorResponse = ApiResponse<User>.Erro("Id deve ser um número positivo", "Validação falhou");
                    errorResponse.CorrelationId = GetCorrelationId();
                    return BadRequest(errorResponse);
                }

                var user = _userService.ObterPorId(id);

                if (user == null)
                {
                    var notFoundResponse = ApiResponse<User>.NotFound($"Usuário com Id {id} não encontrado");
                    notFoundResponse.CorrelationId = GetCorrelationId();
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<User>.SucessoOk(user, "Usuário encontrado com sucesso");
                response.CorrelationId = GetCorrelationId();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário com Id {Id}", id);
                var response = ApiResponse<User>.Erro(ex.Message, "Erro ao obter usuário");
                response.CorrelationId = GetCorrelationId();
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="user">Dados do usuário a ser criado (não incluir Id)</param>
        /// <returns>Usuário criado com Id gerado automaticamente</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<User>> Criar([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    var errorResponse = ApiResponse<User>.Erro("Corpo da requisição não pode estar vazio", "Validação falhou");
                    errorResponse.CorrelationId = GetCorrelationId();
                    return BadRequest(errorResponse);
                }

                var (sucesso, mensagem, userCriado) = _userService.Criar(user);

                if (!sucesso)
                {
                    var errorResponse = ApiResponse<User>.Erro(mensagem, "Erro ao criar usuário");
                    errorResponse.CorrelationId = GetCorrelationId();
                    return BadRequest(errorResponse);
                }

                var response = ApiResponse<User>.SucessoCreate(userCriado, mensagem);
                response.CorrelationId = GetCorrelationId();

                var notificar = _userNotificationService.SendNotificationUser(userCriado, GetCorrelationId());

                return CreatedAtAction(nameof(ObterPorId), new { id = userCriado.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário");
                var response = ApiResponse<User>.Erro(ex.Message, "Erro ao criar usuário");
                response.CorrelationId = GetCorrelationId();
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Altera um usuário existente (partial update)
        /// </summary>
        /// <param name="id">Id do usuário a ser alterado (obrigatório)</param>
        /// <param name="userAtualizado">Dados a serem atualizados. Todos os campos são opcionais - apenas os fornecidos serão alterados.</param>
        /// <returns>Usuário alterado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<User>> Alterar(int id, [FromBody] AtualizarUserDto userAtualizado)
        {
            try
            {
                if (userAtualizado == null)
                {
                    var errorResponse = ApiResponse<User>.Erro("Corpo da requisição não pode estar vazio", "Validação falhou");
                    errorResponse.CorrelationId = GetCorrelationId();
                    return BadRequest(errorResponse);
                }

                if (id <= 0)
                {
                    var errorResponse = ApiResponse<User>.Erro("Id deve ser um número positivo", "Validação falhou");
                    errorResponse.CorrelationId = GetCorrelationId();
                    return BadRequest(errorResponse);
                }

                var (sucesso, mensagem, userAlterado) = _userService.Alterar(id, userAtualizado);

                if (!sucesso)
                {
                    if (mensagem == "Usuário não encontrado")
                    {
                        var notFoundResponse = ApiResponse<User>.NotFound(mensagem);
                        notFoundResponse.CorrelationId = GetCorrelationId();
                        return NotFound(notFoundResponse);
                    }

                    var errorResponse = ApiResponse<User>.Erro(mensagem, "Erro ao alterar usuário");
                    errorResponse.CorrelationId = GetCorrelationId();
                    return BadRequest(errorResponse);
                }

                var response = ApiResponse<User>.SucessoOk(userAlterado, mensagem);
                response.CorrelationId = GetCorrelationId();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar usuário com Id {Id}", id);
                var response = ApiResponse<User>.Erro(ex.Message, "Erro ao alterar usuário");
                response.CorrelationId = GetCorrelationId();
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Exclui um usuário
        /// </summary>
        /// <param name="id">Id do usuário a ser excluído (obrigatório)</param>
        /// <returns>Resultado da exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public ActionResult<ApiResponse<object?>> Excluir(int id)
        {
            try
            {
                if (id <= 0)
                {
                    var errorResponse = ApiResponse<object?>.Erro("Id deve ser um número positivo", "Validação falhou");
                    errorResponse.CorrelationId = GetCorrelationId();
                    return BadRequest(errorResponse);
                }

                var (sucesso, mensagem) = _userService.Excluir(id);

                if (!sucesso)
                {
                    if (mensagem == "Usuário não encontrado")
                    {
                        var notFoundResponse = ApiResponse<object?>.NotFound(mensagem);
                        notFoundResponse.CorrelationId = GetCorrelationId();
                        return NotFound(notFoundResponse);
                    }

                    var errorResponse = ApiResponse<object?>.Erro(mensagem, "Erro ao excluir usuário");
                    errorResponse.CorrelationId = GetCorrelationId();
                    return BadRequest(errorResponse);
                }

                var response = ApiResponse<object?>.SucessoOk(null, mensagem);
                response.CorrelationId = GetCorrelationId();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário com Id {Id}", id);
                var response = ApiResponse<object?>.Erro(ex.Message, "Erro ao excluir usuário");
                response.CorrelationId = GetCorrelationId();
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
