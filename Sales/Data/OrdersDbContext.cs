using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Sales.Models;

namespace Sales.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
    {
        public OrdersDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<OrdersDbContext>();



            builder.UseSqlServer(ConnectionStringSource.GetConnectionString());

            return new OrdersDbContext(builder.Options);
        }
    }

    public static class ConnectionStringSource
    {
        public static string GetConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            return connectionString;
        }
    }

    public class OrdersDbContext : DbContext
    {
        private IDbContextTransaction _currentTransaction;

        public OrdersDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SubmittedOrder> SubmittedOrders { get; set; }
        public DbSet<Color> Colors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionStringSource.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubmittedOrder>().ToTable("SubmittedOrders").Property(x => x.Id).UseSqlServerIdentityColumn();
            modelBuilder.Entity<Color>().ToTable("Colors").Property(x => x.Id).UseSqlServerIdentityColumn();

            modelBuilder.Entity<Color>().HasData(
                new Color {Id = 1, Name = "Red"},
                new Color {Id = 2, Name = "Blue"},
                new Color {Id = 3, Name = "Green"}
                );

        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync().ConfigureAwait(false);

                _currentTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}