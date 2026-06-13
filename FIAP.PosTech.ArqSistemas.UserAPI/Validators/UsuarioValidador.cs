using System.Text.RegularExpressions;

namespace FIAP.PosTech.ArqSistemas.UserAPI.Validators
{
    public static class UsuarioValidador
    {
        /// <summary>
        /// Valida o formato de email
        /// </summary>
        public static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida senha segura: mínimo 8 caracteres, com números, letras e caracteres especiais
        /// </summary>
        public static bool ValidarSenhaSegura(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
                return false;

            // Verifica se tem pelo menos uma letra
            if (!Regex.IsMatch(senha, @"[a-zA-Z]"))
                return false;

            // Verifica se tem pelo menos um número
            if (!Regex.IsMatch(senha, @"[0-9]"))
                return false;

            // Verifica se tem pelo menos um caractere especial (!@#$%^&*-_=+)
            if (!Regex.IsMatch(senha, @"[!@#$%^&*\-_=+]"))
                return false;

            return true;
        }

        /// <summary>
        /// Obtém mensagem de erro de validação de senha
        /// </summary>
        public static string ObterMensagemErroSenha()
        {
            return "A senha deve conter no mínimo 8 caracteres, incluindo números, letras e caracteres especiais (!@#$%^&*-_=+)";
        }
    }
}
