using AutoMapper;
using Music.Core.DTOs;
using Music.Core.Entities;
using Music.Core.IRepositories;
using Music.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Service
{
    public class ConversionProcessService : IConversionProcessService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public ConversionProcessService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ConversionProcessDTO>> GetAllAsync()
        {
            return _mapper.Map<List<ConversionProcessDTO>>(await _repositoryManager.ConversionProcesses.GetAllAsync());
        }
        public async Task<ConversionProcessDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<ConversionProcessDTO>(await _repositoryManager.ConversionProcesses.GetByIdAsync(id));
        }
        public async Task<ConversionProcessDTO> AddAsync(ConversionProcessDTO conversionProcessDto)
        {
            var conversionProcess = _mapper.Map<ConversionProcess>(conversionProcessDto);
            conversionProcessDto = _mapper.Map<ConversionProcessDTO>(await _repositoryManager.ConversionProcesses.AddAsync(conversionProcess));
            await _repositoryManager.SaveAsync();
            return conversionProcessDto;
        }
        public async Task<ConversionProcessDTO> UpdateAsync(int id, ConversionProcessDTO conversionProcessDto)
        {
            var conversionProcess = _mapper.Map<ConversionProcess>(conversionProcessDto);
            conversionProcessDto = _mapper.Map<ConversionProcessDTO>(await _repositoryManager.ConversionProcesses.UpdateAsync(id, conversionProcess));
            await _repositoryManager.SaveAsync();
            return conversionProcessDto;
        }
        public async Task<ConversionProcessDTO> DeleteAsync(int id)
        {
            var conversionProcessDto = _mapper.Map<ConversionProcessDTO>(await _repositoryManager.ConversionProcesses.DeleteAsync(id));
            await _repositoryManager.SaveAsync();
            return conversionProcessDto;
        }
    }
}
