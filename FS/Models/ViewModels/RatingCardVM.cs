using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models.ViewModels
{
    public class RatingCardVM
    {
        public Movie Movies { get; set; }
        public IEnumerable<Rating> Ratings { get; set; }
    }
}
