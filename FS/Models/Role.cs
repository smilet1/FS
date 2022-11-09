using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Название Должности")]
        public string Name { get; set; }
        public ICollection<MovieHuman> MoviePeople { get; set; }
    }
}
