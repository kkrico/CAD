using Cad.Core.Dados.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cad.Core.Dados.Entidades
{
    [Table("USUARIO")]
    public class Usuario : IEntidade
    {
        [Key, Column("Id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Login", TypeName = "varchar"), MaxLength(50)]
        public string Login { get; set; }

        [Column("Senha", TypeName = "varchar"), MaxLength(50)]
        public string Senha { get; set; }

        public bool HasAlteracaoSenha { get; set; }
    }
}