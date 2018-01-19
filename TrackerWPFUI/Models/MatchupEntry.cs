using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerWPFUI.Models
{
    public class MatchupEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? TeamCompetingId { get; set; }

        [ForeignKey("TeamCompetingId")]
        public virtual Team TeamCompeting { get; set; }

        public double? Score { get; set; } = 0;

        public int MatchupId { get; set; }

        [ForeignKey("MatchupId")]
        public virtual Matchup Matchup { get; set; }

        public int? ParentMatchupId { get; set; }

        [ForeignKey("ParentMatchupId")]
        public virtual Matchup ParentMatchup { get; set; }
    }
}
