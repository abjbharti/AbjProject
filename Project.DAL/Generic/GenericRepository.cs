using Microsoft.EntityFrameworkCore;
using Project.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ProjectDbContext _context;
        private readonly DbSet<T> entities;

        public GenericRepository(ProjectDbContext projectDbContext)
        {
            _context = projectDbContext;
            entities = projectDbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await entities.ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await entities.FindAsync(id);
        }

        public async Task Insert(T obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            await entities.AddAsync(obj);
            await Save();
        }

        public async Task Update(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            entities.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
            await Save();
        }

        public async Task<T> Delete(object id)
        {
            var entity = await entities.FindAsync(id);

            if (entity != null)
            {
                entities.Remove(entity);
                await Save();
            }
            return entity;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
