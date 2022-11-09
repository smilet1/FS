using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Название")]
        public string Title { get; set; }
        [DisplayName("Слоган")]
        public string Tagline { get; set; }
        [DisplayName("Дата Релиза")]
        public DateTime Release { get; set; }
        [Required]
        [DisplayName("Длительность")]
        public int Duration { get; set; }
        [Required]
        [DisplayName("Описание")]
        public string Description { get; set; }
        [DisplayName("Постер")]
        public string Img { get; set; }
        public ICollection<Compilation> Compilations { get; set; }
        public ICollection<Country> Countries { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<MovieHuman> MoviePeople { get; set; }
        //public Movie()
        //{
        //    Genres = new List<Genre>();
        //}
    }
}
