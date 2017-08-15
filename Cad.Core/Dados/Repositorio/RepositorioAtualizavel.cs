using Cad.Core.Dados.Interface;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Cad.Core.Dados.Repositorio
{
    public class RepositorioAtualizavel<TEntidade>
        : Repositorio<TEntidade>, IRepositorioAtualizavel<TEntidade> where TEntidade : class, IEntidade
    {
        public RepositorioAtualizavel(CADContext contexto) : base(contexto) { }

        public void AdicionarOuAtualizar(TEntidade entidade)
        {
            Set.AddOrUpdate(entidade);
        }

        public void Adicionar(TEntidade entidade)
        {
            Set.Add(entidade);
        }

        public void Atualizar(TEntidade entidade)
        {
            Context.Entry(entidade).State = EntityState.Modified;
        }

        public void Excluir(TEntidade entidade)
        {
            Set.Remove(entidade);
        }

        public void Excluir(int id)
        {
            var entidade = Obter(id);

            Set.Remove(entidade);
        }

        public void SalvarAlteracoes()
        {
            Context.SaveChanges();
        }
    }
}
