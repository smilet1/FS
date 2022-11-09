using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models
{
    public class Rating
    {
        [Key]
        public string UserName { get; set; }
        [Key]
        public int MovieId { get; set; }
        [Required]
        public int UserRating { get; set; }
        public string Comment { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
