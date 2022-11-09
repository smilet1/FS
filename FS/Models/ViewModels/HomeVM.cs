using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<RatingCardVM> Movies { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public IEnumerable<Compilation> Compilations { get; set; }
    }
}
