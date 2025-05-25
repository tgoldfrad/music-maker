using Microsoft.EntityFrameworkCore;
using Music.Core.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Music.Core.Entities.File;

namespace Music.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> UserList { get; set; }
        public DbSet<File> FileList { get; set; }
        public DbSet<ConversionProcess> ConversionProcessList { get; set; }
        public DbSet<Role> RoleList { get; set; }
        public DbSet<Permission> PermissonList { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity(j => j.ToTable("RolePermissions"));
        }
        //var connectionString = "server=bgb9ojh8gaxhvg5hmvb9-mysql.services.clever-cloud.com;user=usjr0zjxfvslhjvf;password=ZlbVNmJxxCriv0AuAAMw;database=bgb9ojh8gaxhvg5hmvb9;";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            optionsBuilder.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString));
        }
        //public DataContext(DbContextOptions<DataContext> options) : base(options)
        //{

        //}
    }
}
