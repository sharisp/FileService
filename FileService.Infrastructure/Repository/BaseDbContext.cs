using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileService.Domain.Entity;

namespace FileService.Infrastructure.Repository
{
    public class BaseDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options)
            : base(options)
        {
        }

        public DbSet<FileUpload> FileUploads { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);

            // Add any custom configurations here
            // For example, you can configure entity properties, relationships, etc.
        }
    }
}
