using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace VideoGameStatsDemo.Models
{
    public class GameConsole
    {
    
   
      
        [Display(Name ="Console")]
        [Key]
        public string? ConsoleName { get; set; }
        public string? Manufacturer { get; set; }
        [Display(Name = "Release Year")]
        [Range(1980,2022)]
        public int? ReleaseYear { get; set; }
        [Display(Name = "Total Sold (in Millions)")]
        [Range(0, 10000000000)]
        public double? ConsoleSold { get; set; }

        public virtual ICollection<Game>? Games { get; set; }

    }
}
