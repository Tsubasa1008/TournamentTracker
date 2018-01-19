using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerWPFUI.Models
{
    public class TournamentsTestContext : DbContext
    {
        public TournamentsTestContext()
            : base("name=TournamentsTestContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TournamentsTestContext, TrackerWPFUI.Migrations.Configuration>("TournamentsTestContext"));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prize>().Property(x => x.PrizeAmount).HasPrecision(10, 2);

            modelBuilder.Entity<Tournament>().Property(x => x.EntryFee).HasPrecision(10, 2);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Tournament> Tournaments { get; set; }

        public virtual DbSet<Matchup> Matchups { get; set; }

        public virtual DbSet<MatchupEntry> MatchupEntries { get; set; }

        /// <summary>
        /// Profile Table
        /// </summary>
        public virtual DbSet<People> People { get; set; }

        /// <summary>
        /// Teams Table
        /// </summary>
        public virtual DbSet<Team> Teams { get; set; }

        /// <summary>
        /// Prizes Table
        /// </summary>
        public virtual DbSet<Prize> Prizes { get; set; }
    }
}
