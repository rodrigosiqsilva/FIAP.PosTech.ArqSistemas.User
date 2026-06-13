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
    /// DTO para atualizar um usuário existente (partial update).
    /// Todos os campos são opcionais - apenas os campos fornecidos serão validados e atualizados.
    /// </summary>
    public class AtualizarUsuarioDto
    {
        /// <summary>
        /// Nome do usuário. Opcional - se omitido, não será alterado.
        /// </summary>
        public string? Nome { get; set; }

        /// <summary>
        /// Email do usuário. Opcional - se omitido, não será alterado.
        /// Deve ser um email válido se fornecido.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Senha do usuário. Opcional - se omitido, não será alterada.
        /// Se fornecida, deve atender aos requisitos de segurança (mín. 8 caracteres, letra, dígito, caractere especial).
        /// </summary>
        public string? Senha { get; set; }
    }

    /// <summary>
    /// DTO para autenticar um usuário.
    /// </summary>
    public class LoginUsuarioDto
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
