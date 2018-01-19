using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerWPFUI.Models
{
    public class Team
    {
        public Team()
        {
            TeamMembers = new HashSet<People>();
            Tournaments = new HashSet<Tournament>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string TeamName { get; set; }

        public virtual ICollection<People> TeamMembers { get; set; }

        public virtual ICollection<Tournament> Tournaments { get; set; }
    }
}
