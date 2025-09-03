using Microsoft.EntityFrameworkCore;
using Quizzz.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Quizzz.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly QuizzContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(QuizzContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.FirstOrDefaultAsync(filter);
        }

        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> filter, bool useNoTracking)
        {
            if (useNoTracking)
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter);
            else
                return await _dbSet.FirstOrDefaultAsync(filter);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new Exception("Property 'Id' not found on type " + typeof(T).Name);

            var idValue = idProperty.GetValue(entity);
            var existingEntity = await _dbSet.FindAsync(idValue);
            if (existingEntity == null)
                throw new Exception("Entity not found");

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T> GetByUsernameAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.FirstOrDefaultAsync(filter);
        }
        

    }
}
