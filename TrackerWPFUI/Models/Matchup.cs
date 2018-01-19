using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerWPFUI.Models
{
    public class Matchup
    {
        public Matchup()
        {
            Entries = new HashSet<MatchupEntry>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TournamentId { get; set; }

        [ForeignKey("TournamentId")]
        public virtual Tournament Tournament { get; set; }

        public int? WinnerId { get; set; }

        [ForeignKey("WinnerId")]
        public virtual Team Winner { get; set; }

        [Required]
        public int MatchupRound { get; set; }

        [ForeignKey("MatchupId")]
        public virtual ICollection<MatchupEntry> Entries { get; set; }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                string output = "";

                foreach (MatchupEntry me in Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = me.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs. { me.TeamCompeting.TeamName }";
                        }
                    }
                    else
                    {
                        output = "Matchup Not Yet Determined";
                        break;
                    }
                }

                return output;
            }
        }
    }
}
