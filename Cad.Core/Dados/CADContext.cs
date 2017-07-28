using Cad.Core.Dados.Entidades;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Cad.Core.Dados
{
    public class CADContext : DbContext
    {
        public CADContext() : this("name=ConexaoCAD")
        {

        }

        public CADContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }
    }
}
