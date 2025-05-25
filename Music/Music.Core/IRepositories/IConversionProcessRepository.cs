using Music.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.IRepositories
{
    public interface IConversionProcessRepository
    {
        Task<IEnumerable<ConversionProcess>> GetAllAsync();
        Task<ConversionProcess> GetByIdAsync(int id);
        Task<ConversionProcess> AddAsync(ConversionProcess conversionProcess);
        Task<ConversionProcess> UpdateAsync(int id, ConversionProcess conversionProcess);
        Task<ConversionProcess> DeleteAsync(int id);
    }
}
