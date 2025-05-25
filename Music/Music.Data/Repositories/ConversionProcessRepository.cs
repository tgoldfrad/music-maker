using Microsoft.EntityFrameworkCore;
using Music.Core.Entities;
using Music.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Data.Repositories
{
    public class ConversionProcessRepository : IConversionProcessRepository
    {
        private readonly DataContext _dataContext;
        private readonly DbSet<ConversionProcess> _conversionProcesses;
        public ConversionProcessRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _conversionProcesses = _dataContext.ConversionProcessList;
        }
        public async Task<IEnumerable<ConversionProcess>> GetAllAsync()
        {
            return await _conversionProcesses.ToListAsync();
        }

        public async Task<ConversionProcess> GetByIdAsync(int id)
        {
            return await _conversionProcesses.FindAsync(id);
        }

        public async Task<ConversionProcess> AddAsync(ConversionProcess conversionProcess)
        {
            var c = await _conversionProcesses.AddAsync(conversionProcess);
            return c.Entity;
        }
        public async Task<ConversionProcess> UpdateAsync(int id, ConversionProcess conversionProcess)
        {
            var conversionProcessToUpdate = await _conversionProcesses.FindAsync(id);//בטוח לא null
            ///////////////////אין מה  לשנות


            //conversionProcessToUpdate.UpdatedAt = DateTime.Now;

            //productToUpdate.ProductName = !String.IsNullOrEmpty(product.ProductName) ? product.ProductName : productToUpdate.ProductName;

            //return _dataContext.SaveChange(dataProduct);

            return conversionProcessToUpdate;
        }
        public async Task<ConversionProcess> DeleteAsync(int id)
        {
            var conversionProcessToDelete = await _conversionProcesses.FindAsync(id);
            await Task.Run(() => _conversionProcesses.Remove(conversionProcessToDelete));
            return conversionProcessToDelete;
        }
    }
}
