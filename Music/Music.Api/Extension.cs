using Microsoft.EntityFrameworkCore;
using Music.Core;
using Music.Core.IRepositories;
using Music.Core.IServices;
using Music.Data;
using Music.Data.Repositories;
using Music.Service;

namespace Music.Api
{
    public static class Extension
    {
        public static void ServiceDependencyInjector(this IServiceCollection services)
        {

            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IConversionProcessRepository, ConversionProcessRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IConversionProcessService, ConversionProcessService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAWSService, AWSService>();

            services.AddAutoMapper(typeof(MappingPostProfile));
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddDbContext<DataContext>();

        }
    }
}
