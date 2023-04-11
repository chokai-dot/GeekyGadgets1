using Automarket.DAL;
using GeekyGadgets.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.DAL.Repositories
{
    public class SmartphoneRepository
    {
        private readonly ApplicationDbContext _db;

        public SmartphoneRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Create(Smartphone entity)
        {
            await _db.Smartphones.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Smartphone> GetAll()
        {
            return _db.Smartphones;
        }

        public async Task Delete(Smartphone entity)
        {
            _db.Smartphones.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<Smartphone> Update(Smartphone entity)
        {
            _db.Smartphones.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }
    }
}
