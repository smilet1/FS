using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models
{
    public class MovieHuman
    {
        [Key]
        [DisplayName("Полное Имя")]
        public int HumanId { get; set; }
        [Key]
        [DisplayName("Фильм")]
        public int MovieId { get; set; }
        [Key]
        [DisplayName("Должность")]
        public int RoleId { get; set; }
        public virtual Human Human { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
