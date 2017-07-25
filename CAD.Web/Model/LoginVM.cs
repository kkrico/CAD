using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CAD.Web.Model
{
    public class LoginVM
    {
        [Display(Name = "Login")]
        [Required]
        [CPF]
        public string Login { get; set; }

        [Display(Name = "Senha")]
        [Required]
        public string Senha { get; set; }

        [Compare("Senha")]
        public string ConfirmacaoSenha { get; set; }

        [Display(Name = "Lembrar a senha?")]
        public bool LembrarSenha { get; set; }
    }

    public class CPFAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (!(value is string)) return false;

            var cpf = (string)value;
            var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (var i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            var resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            var digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto;

            return cpf.EndsWith(digito);
        }
    }
}