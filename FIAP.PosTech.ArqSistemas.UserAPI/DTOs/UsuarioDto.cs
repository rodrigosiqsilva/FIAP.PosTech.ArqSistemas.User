namespace FIAP.PosTech.ArqSistemas.UserAPI.DTOs
{
    /// <summary>
    /// DTO para criar um novo usuário.
    /// Não deve conter Id (será gerado automaticamente no servidor)
    /// </summary>
    public class CriarUsuarioDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    /// <summary>
    /// DTO para atualizar um usuário existente
    /// </summary>
    public class AtualizarUsuarioDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
