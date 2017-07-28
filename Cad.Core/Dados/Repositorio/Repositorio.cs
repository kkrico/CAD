using Cad.Core.Dados.Interface;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Cad.Core.Dados.Repositorio
{
    public class Repositorio<TEntidade> : IRepositorio<TEntidade> where TEntidade : class, IEntidade
    {
        protected CADContext _context;
        protected DbSet<TEntidade> _set;

        public Repositorio()
        {
            _context = new CADContext();
            _set = _context.Set<TEntidade>();
        }

        public Repositorio(CADContext contexto)
        {
            _context = contexto;
            _set = _context.Set<TEntidade>();
        }

        public virtual TEntidade Obter(int id)
        {
            return _set.Find(id);
        }

        public virtual TEntidade Obter(Expression<Func<TEntidade, bool>> criterio)
        {
            return _set.Where(criterio).FirstOrDefault();
        }

        public virtual TEntidade ObterUltimo(Expression<Func<TEntidade, bool>> criterio)
        {
            return _set.Where(criterio).OrderByDescending(a => a.Id).FirstOrDefault();
        }

        public IQueryable<TEntidade> Buscar(Expression<Func<TEntidade, bool>> criterio,
            Func<TEntidade, object> ordenarPor = null,
            params Expression<Func<TEntidade, object>>[] propriedadesDeNavegacao)
        {
            IQueryable<TEntidade> set = _set;

            IncluirPropriedadesNaQuery(propriedadesDeNavegacao, set);

            IQueryable<TEntidade> resultado = ordenarPor != null ?
                set.Where(criterio).OrderBy(ordenarPor).AsQueryable() :
                set.Where(criterio).AsQueryable();


            return resultado;
        }

        public IQueryable<TEntidade> Todos()
        {
            return _set;
        }

        public bool Existe(int id)
        {
            return _set.Any(a => a.Id == id);
        }

        public bool Existe(Expression<Func<TEntidade, bool>> criterio)
        {
            return _set.Any(criterio);
        }

        public int Contar(Expression<Func<TEntidade, bool>> criterio)
        {
            return _set.Count(criterio);
        }

        private void IncluirPropriedadesNaQuery(Expression<Func<TEntidade, object>>[] propriedadesDeNavegacao, IQueryable<TEntidade> entidade)
        {
            propriedadesDeNavegacao.Aggregate(entidade, (current, propriedade) => current.Include(propriedade));
        }
    }
}
