using FileService.Domain.Entity;
using Infrastructure.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FileService.Infrastructure.Repository
{
    public class AppDbContext : BaseDbContext
    {
        // 不能直接用DbContextOptions options

        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        }

        public DbSet<FileUpload> FileUploads { get; set; }



    }
}
