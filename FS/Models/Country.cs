using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Название Страны")]
        public string Name { get; set; }
        public ICollection<Movie> Movies { get; set; }
    }
}
