using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Model;

namespace DAL
{
    public class DbAccess : DbContext
    {
        public DbAccess(DbContextOptions<DbAccess> dbContextOption) : base(dbContextOption)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }

            }
            catch (Exception ex)
            {
                // Write to log
                throw new Exception(ex.ToString());
            }

        }

        
        public DbSet<M_UniqueIds> Unique_Ids { get; set; }
        public DbSet<M_User> Allowd_User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<M_UniqueIds>()
                .HasKey(e => new { e.Scope, e.Name, e.ID });

            // Defining unique constraints

            // M_UniqueIds
            modelBuilder.Entity<M_UniqueIds>()
                .HasIndex(e => new { e.Scope, e.Name })
                .IsUnique();

            modelBuilder.Entity<M_UniqueIds>()
                .HasIndex(e => new { e.Scope, e.ID })
                .IsUnique();

            // M_User
            modelBuilder.Entity<M_User>()
            .HasIndex(u => u.UserID)
            .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

    }


}