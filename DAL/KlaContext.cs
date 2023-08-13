using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Model;
using Utility_LOG;

namespace DAL
{
    public class KlaContext : DbContext
    {
        private readonly LogManager _log;

        public KlaContext(DbContextOptions<KlaContext> dbContextOption, LogManager log) : base(dbContextOption)
        {
            _log = log;

            try
            {
                ChangeTracker.LazyLoadingEnabled = false;

                

                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }


            }
            catch (Exception ex)
            {
                _log.LogException(ex.Message, ex, LogProviderType.File);
                throw;
            }
        }

        // For Tests
		public KlaContext() : base()
		{
			
		}

		public DbSet<User> Users{ get; set; }

        public DbSet<UniqueIds> Unique_Ids { get; set; }

        public DbSet<Aliases> Aliases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UniqueIds>()
                .HasKey(e => new { e.Scope, e.Name, e.ID });

            modelBuilder.Entity<UniqueIds>()
                .HasKey(e => new { e.Scope, e.ID });

            // M_UniqueIds
            modelBuilder.Entity<UniqueIds>()
                .HasIndex(e => new { e.Scope, e.Name })
                .IsUnique();

            modelBuilder.Entity<UniqueIds>()
                .HasIndex(e => new { e.Scope, e.ID })
                .IsUnique();

            // M_User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserID)
                .IsUnique();

            // Aliases
            modelBuilder.Entity<Aliases>(entity =>
            {
                entity.HasKey(a => new { a.ID, a.Scope, a.CurrentAliasName });

                entity.HasOne(a => a.UniqueId)
                    .WithMany(u => u.Aliases)
                    .HasForeignKey(a => new { a.Scope, a.ID });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}