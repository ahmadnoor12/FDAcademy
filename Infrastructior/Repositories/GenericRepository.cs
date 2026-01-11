using Application.RepositoriesInterface;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly FDAcademyDbContext _fDADbContext;
        public GenericRepository(FDAcademyDbContext fDADbContext)
        {
            _fDADbContext = fDADbContext;
        }
        public async Task<T> GetById(int id)
        {
            return await _fDADbContext.Set<T>().FindAsync(id);
        }
        public async Task Insert(T entity)
        {
            await _fDADbContext.Set<T>().AddAsync(entity);
        }
        public async Task InsertRange(List<T> entity)
        {
            await _fDADbContext.Set<T>().AddRangeAsync(entity);
        }
        public IQueryable<T> GetAll()
        {
            return _fDADbContext.Set<T>();
        }
        public void Update(T entity)
        {
            _fDADbContext.Set<T>().Update(entity);
        }
        public async Task Delete(T entity)
        {
            _fDADbContext.Set<T>().Remove(entity);
        }
        public async Task<int> SaveChanges()
        {
            return await _fDADbContext.SaveChangesAsync();
        }

        public async Task DeleteRange(List<T> entity)
        {
            _fDADbContext.Set<T>().RemoveRange(entity);
        }
    }
}
