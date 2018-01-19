using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerWPFUI.Models
{
    public class Tournament
    {
        public Tournament()
        {
            EnteredTeams = new HashSet<Team>();
            //Prizes = new HashSet<Prize>();
            Matchups = new HashSet<Matchup>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool Active { get; set; } = true;
        /// <summary>
        /// varchar(100)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string TournamentName { get; set; }

        /// <summary>
        /// decimal(10, 2)
        /// </summary>
        [Required]
        public decimal EntryFee { get; set; } = 0;

        public virtual ICollection<Team> EnteredTeams { get; set; }

        //public virtual ICollection<Prize> Prizes { get; set; }

        public virtual ICollection<Matchup> Matchups { get; set; }

        public event EventHandler<DateTime> OnTournamentComplete;

        public void CompleteTournament()
        {
            OnTournamentComplete?.Invoke(this, DateTime.Now);
        }
    }
}
