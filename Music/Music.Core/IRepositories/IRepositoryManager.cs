using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.IRepositories
{
    public interface IRepositoryManager
    {
        IUserRepository Users { get; }
        IFileRepository Files { get; }
        IConversionProcessRepository ConversionProcesses { get; }
        IRoleRepository Roles { get; }

        Task SaveAsync();
    }
}
