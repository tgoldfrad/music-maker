using Music.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.IServices
{
    public interface IConversionProcessService
    {
        Task<IEnumerable<ConversionProcessDTO>> GetAllAsync();
        Task<ConversionProcessDTO> GetByIdAsync(int id);
        Task<ConversionProcessDTO> AddAsync(ConversionProcessDTO conversionProcess);
        Task<ConversionProcessDTO> UpdateAsync(int id, ConversionProcessDTO conversionProcess);
        Task<ConversionProcessDTO> DeleteAsync(int id);
    }
}
