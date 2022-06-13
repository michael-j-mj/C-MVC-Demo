using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameStatsDemo.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Game
    {
        [Key]
      
        public int Id { get; set; }
        [Required]

        [StringLength(50)]
        public string Name { get; set; }
    
        [Display(Name = "Console")]
        public string Catagory { get; set; }
        [ForeignKey("Catagory")]
        public virtual GameConsole? Console { get; set; }
       //public virtual GameConsole GameConsole { get; set; }
        [Required]
        [Range(0, 1000)]
        [Display(Name = "Total Sold (in Millions)")]
        public double? Value { get; set; }
        

            
    }
}
