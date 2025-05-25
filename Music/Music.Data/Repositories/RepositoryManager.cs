using Music.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Data.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly DataContext _dataContext;
        public IUserRepository Users { get; }
        public IFileRepository Files { get; }
        public IConversionProcessRepository ConversionProcesses { get; }
        public IRoleRepository Roles { get; }
        public RepositoryManager(DataContext dataContext, IUserRepository users, IFileRepository files, IConversionProcessRepository conversionProcesses, IRoleRepository roles)
        {
            _dataContext = dataContext;
            Users = users;
            Files = files;
            ConversionProcesses = conversionProcesses;
            Roles = roles;
        }
        public async Task SaveAsync()
        {
            await _dataContext.SaveChangesAsync();
        }
    }
}
