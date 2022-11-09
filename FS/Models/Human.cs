using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models
{
    public class Human
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Полное Имя")]
        public string FullName { get; set; }
        [Required]
        [DisplayName("Дата Рождения")]
        public DateTime BirthDate { get; set; }
        [DisplayName("Дата Смерти")]
        public DateTime DeathDate { get; set; }
        [DisplayName("Фото")]
        public string Img { get; set; }
        public ICollection<MovieHuman> MoviePeople { get; set; }
    }
}
