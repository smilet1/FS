using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Models.ViewModels
{
    public class MovieVM
    {
        public Movie Movie { get; set; }
        public MovieHuman MovieHuman { get; set; }
        public IEnumerable<SelectListItem> GenreSelectList { get; set; }
        public IEnumerable<SelectListItem> CountrySelectList { get; set; }
        public IEnumerable<SelectListItem> CompilationSelectList { get; set; }
        public IEnumerable<SelectListItem> HumanSelectList { get; set; }
        public IEnumerable<SelectListItem> RoleSelectList { get; set; }
        public IEnumerable<MoviePeopleVM> MovieHumanList { get; set; }

    }
}
