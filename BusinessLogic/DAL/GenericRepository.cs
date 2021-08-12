using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.DAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IId
    {
        protected readonly MeetingRoomContext _context;
        protected readonly DbSet<TEntity> dbset;

        public GenericRepository(MeetingRoomContext context)
        {
            _context = context;
            dbset = _context.Set<TEntity>();
        }

        private bool disposedValue;

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AddAsync)} could not be saved: {ex.Message}");
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                return await dbset.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated: {ex.Message}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = await dbset.FindAsync(id);

            if (entity == null)
            {
                throw new Exception($"Entity with id {id} doesn't exist");
            }

            return entity;
        }

        public async Task<TEntity> Delete(int id)
        {
            var entity = await GetByIdAsync(id);

            dbset.Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public IQueryable<TEntity> Query()
        {
            return dbset.AsQueryable();
        }
    }
}
