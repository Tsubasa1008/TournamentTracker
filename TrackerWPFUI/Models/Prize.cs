using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TrackerWPFUI.Models
{
    public class Prize
    {
        /// <summary>
        /// int(11)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// int(11)
        /// </summary>
        [Required]
        public int PlaceNumber { get; set; }

        /// <summary>
        /// varchar(50)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string PlaceName { get; set; }

        /// <summary>
        /// decimal(10, 2)
        /// </summary>
        public decimal PrizeAmount { get; set; } = 0;

        /// <summary>
        /// double(5, 2)
        /// </summary>
        public double PrizePercentage { get; set; } = 0;

        //public int? TournamentId { get; set; }

        //[ForeignKey("TournamentId")]
        //public virtual Tournament Tournament { get; set; }
    }
}
